using AuthService.Core.Services.DTOs;
using AuthService.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MonitoringService;
using OpenTelemetry.Trace;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly Tracer _tracer;
    
    public AuthController(IAuthService authService, Tracer tracer)
    {
        _authService = authService;
        _tracer = tracer;
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] CreateAuthDto dto)
    {
        using var activity = _tracer.StartActiveSpan("Register");

        try
        {
            LoggingService.Log.Information("Called Register Method");
            await _authService.Register(dto);
            return StatusCode(201, "Successfully registered");
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            return BadRequest(e.ToString());
        }
    }
    
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        using var activity = _tracer.StartActiveSpan("Login");

        try
        {
            LoggingService.Log.Information("Called Login method");
 
            return Ok(await _authService.Login(dto));
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            return BadRequest(e.ToString());
        }
    }
    
    [HttpGet]
    [Route("ValidateToken")]
    public async Task<bool> ValidateToken()
    {
        using var activity = _tracer.StartActiveSpan("ValidateToken");

        try
        {
            // checking for a valid token in the Authorization header
            var re = Request;

            if (!re.Headers.ContainsKey("Authorization"))
            {
                LoggingService.Log.Error("Called Validate Method No Bearer Token");
                return await Task.Run(() => false);
            }


            if (!re.Headers["Authorization"].ToString().StartsWith("Bearer "))
            {
                LoggingService.Log.Error("Called Validate Method No Bearer Token");
                return await Task.Run(() => false);
            }
            
            LoggingService.Log.Information("Called Validate Method");
            
            // decode the token & check if it's valid
            var token = re.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _authService.ValidateToken(token);
            return result.Succeeded;
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            return await Task.Run(() => false);
        }
    }
    
    [HttpPost]
    [Route("Rebuild")]
    public async Task<IActionResult> RebuildDatabase()
    {
        using var activity = _tracer.StartActiveSpan("RebuildDatabase");

        try
        {
            LoggingService.Log.Information("Called RebuildDatabase Method");
            await _authService.RebuildDatabase();
            return Ok();
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            return BadRequest(e.Message);
        }
    }
    
}