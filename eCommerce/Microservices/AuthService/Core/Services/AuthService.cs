using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthService.Core.Entities;
using AuthService.Core.Repositories.Interfaces;
using AuthService.Core.Services.DTOs;
using AuthService.Core.Services.Interfaces;
using AutoMapper;
using Messaging;
using Messaging.SharedMessages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MonitoringService;
using OpenTelemetry.Trace;

namespace AuthService.Core.Services;

public class AuthService : IAuthService
{
    private readonly IMapper _mapper;
    private readonly IAuthRepository _authRepository;
    private readonly MessageClient _messageClient;
    private readonly AppSettings.AppSettings _appSettings;
    private readonly Tracer _tracer;


    public AuthService(IAuthRepository authRepository, MessageClient messageClient,
        IOptions<AppSettings.AppSettings> appSettings, IMapper mapper, Tracer tracer)
    {
        _authRepository = authRepository;
        _messageClient = messageClient;
        _mapper = mapper;
        _appSettings = appSettings.Value;
        _tracer = tracer;
    }

    public async Task Register(CreateAuthDto auth)
    {
        using var activity = _tracer.StartActiveSpan("Register");
        LoggingService.Log.Information("Called Register Method");

        var exist = await _authRepository.DoesAuthExists(auth.Email);

        if (exist)
            throw new DuplicateNameException($"{auth.Email} is already in use");

        var saltBytes = new byte[32];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);

        var salt = Convert.ToBase64String(saltBytes);

        var newAuth = new Auth
        {
            Email = auth.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(auth.Password + salt),
            Salt = salt,
            CreatedAt = DateTime.Now
        };

        try
        {
            var user = await _authRepository.Register(newAuth);
            const string exchangeName = "CreateUserExchange";
            const string routingKey = "CreateUser";

            _messageClient.Send(
                new CreateUserMessage("Create user message", user.Email, user.PasswordHash, user.CreatedAt),
                exchangeName, routingKey);
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            throw new ArgumentException(e.Message);
        }
    }

    public async Task DeleteAuth(int authId)
    {
        using var activity = _tracer.StartActiveSpan("DeleteAuth");

        try
        {
            LoggingService.Log.Information("Called DeleteAuth method");

            var auth = await _authRepository.GetAuthById(authId);
            if (auth is null)
                throw new KeyNotFoundException($"No user with id of {authId}");
            await _authRepository.DeleteAuth(authId);
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            throw new ArgumentException(e.Message);
        }
    }

    public async Task<AuthenticationToken> Login(LoginDto login)
    {
        using var activity = _tracer.StartActiveSpan("Login");

        try
        {
            LoggingService.Log.Information("Called Login method");

            var loggedInUser = await _authRepository.GetAuthByEmail(login.Email);
            if (loggedInUser == null) throw new Exception("Invalid login");

            if (BCrypt.Net.BCrypt.Verify(login.Password + loggedInUser.Salt, loggedInUser.PasswordHash))
            {
                return GenerateToken(loggedInUser);
            }
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            throw new ArgumentException(e.Message);
        }

        throw new Exception("Invalid login");
    }

    public async Task<AuthenticateResult> ValidateToken(string token)
    {
        using var activity = _tracer.StartActiveSpan("ValidateToken");

        if (token.IsNullOrEmpty()) return await Task.Run(() => AuthenticateResult.Fail("Invalid token"));

        try
        {
            LoggingService.Log.Information("Called ValidateToken method");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            }, out var validatedToken);


            if (principal == null)
            {
                return AuthenticateResult.Fail("Invalid token");
            }

            var claims = new List<Claim>();
            foreach (var claim in principal.Claims)
            {
                claims.Add(new Claim(claim.Type, claim.Value));
            }

            var claimsIdentity = new ClaimsIdentity(claims, "dev");
            var claimPrincipal = new ClaimsPrincipal(claimsIdentity);
            var ticket = new AuthenticationTicket(claimPrincipal, "dev");
            return AuthenticateResult.Success(ticket);
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            return await Task.Run(() => AuthenticateResult.Fail("Invalid token"));
        }
    }

    public async Task<GetAuthDto> GetAuthById(int id)
    {
        using var activity = _tracer.StartActiveSpan("GetAuthById");

        if (id < 1)
            throw new ArgumentException("Id could be less than 0");

        try
        {
            LoggingService.Log.Information("Called GetAuthById Method");

            var user = _mapper.Map<GetAuthDto>(await _authRepository.GetAuthById(id));

            return user;
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            throw new ArgumentException(e.Message);
        }
    }


    private AuthenticationToken GenerateToken(Auth auth)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new("Id", auth.Id.ToString()),
            new("Email", auth.Email),
        };

        var tokenOptions = new JwtSecurityToken(
            signingCredentials: signingCredentials,
            claims: claims,
            expires: DateTime.Now.AddMinutes(5)
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        var authToken = new AuthenticationToken
        {
            Value = tokenString
        };

        return authToken;
    }

    public async Task RebuildDatabase()
    {
        using var activity = _tracer.StartActiveSpan("Rebuild Auth Database");

        try
        {
            LoggingService.Log.Information("Called Rebuild Auth Database Method");
            await _authRepository.RebuildDatabase();
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            throw new ArgumentException(e.Message);
        }
    }
}