using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Core.Entities;
using AuthService.Core.Repositories.Interfaces;
using AuthService.Core.Services.DTOs;
using AuthService.Core.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Core.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IMapper _mapper;
    
    private const string SecurityKey = "Secret"; //TODO 


    public AuthService(IAuthRepository authRepository, IMapper mapper)
    {
        _authRepository = authRepository;
        _mapper = mapper;
    }
    public async Task Register(CreateAuthDto auth)
    {
        var exist = await _authRepository.DoesAuthExists(auth.Email);

        if (exist)
            throw new DuplicateNameException($"{auth.Email} is already in use");

        await _authRepository.Register(_mapper.Map<Auth>(auth));
    }

    public async Task DeleteAuth(int authId)
    {
        var auth = await _authRepository.GetAuthById(authId);
        if (auth is null)
            throw new KeyNotFoundException($"No user with id of {authId}");
        await _authRepository.DeleteAuth(authId);
    }
    
    public async Task<AuthenticationToken> Login(LoginDto login)
    {
        var loggedInUser = await _authRepository.GetAuthByEmail(login.Email);
        if (loggedInUser == null) throw new Exception("Invalid login");

        if (BCrypt.Net.BCrypt.Verify(login.Password + loggedInUser.Salt, loggedInUser.PasswordHash))
        {
            return GenerateToken(loggedInUser);
        }

        throw new Exception("Invalid login");
    }

    public async Task<AuthenticateResult> ValidateToken(string token)
    {
        if (token.IsNullOrEmpty()) return await Task.Run(() => AuthenticateResult.Fail("Invalid token"));

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecurityKey);
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
        catch
        {
            return await Task.Run(() => AuthenticateResult.Fail("Invalid token"));
        }
    }
    private AuthenticationToken GenerateToken(Auth auth)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new ("Id", auth.Id.ToString()),
            new ("Email", auth.Email),
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
}