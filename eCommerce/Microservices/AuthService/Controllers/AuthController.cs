using AuthService.Core.Services.DTOs;
using AuthService.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] CreateAuthDto dto)
    {
        try
        {
            await _authService.Register(dto);
            return StatusCode(201, "Successfully registered");
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }
    
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            return Ok(await _authService.Login(dto));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet]
    [Route("ValidateToken")]
    public async Task<bool> ValidateToken()
    {
        try
        {
            // checking for a valid token in the Authorization header
            var re = Request;

            if (!re.Headers.ContainsKey("Authorization"))
                return await Task.Run(() => false);

            if (!re.Headers["Authorization"].ToString().StartsWith("Bearer "))
                return await Task.Run(() => false);

            
            // decode the token & check if it's valid
            var token = re.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _authService.ValidateToken(token);
            return result.Succeeded;
        }
        catch (Exception e)
        {
            return await Task.Run(() => false);
        }
    }
    
    [HttpPost]
    [Route("Rebuild")]
    public async Task<IActionResult> RebuildDatabase()
    {
        try
        {
            await _authService.RebuildDatabase();
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
}