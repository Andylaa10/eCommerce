using Microsoft.AspNetCore.Mvc;
using MonitoringService;
using OpenTelemetry.Trace;
using UserService.Core.Services.DTOs;
using UserService.Core.Services.Interfaces;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly Tracer _tracer;
    
    public UserController(IUserService userService, Tracer tracer)
    {
        _userService = userService;
        _tracer = tracer;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        using var activity = _tracer.StartActiveSpan("CreateUser");
        
        try
        {
            LoggingService.Log.Information("Called CreateUser Method");
            return StatusCode(201, await _userService.AddUser(dto));
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            return BadRequest(e.ToString());
        }
    }
    
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetUserById([FromRoute] int id)
    {
        using var activity = _tracer.StartActiveSpan("GetUserById");

        try
        {
            LoggingService.Log.Information("Called GetUserById Method");
            return Ok(await _userService.GetUserById(id));
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            return BadRequest(e.ToString());
        }
    }
    
    [HttpGet]
    [Route("Users")]
    public async Task<IActionResult> GetAllUsers()
    {
        using var activity = _tracer.StartActiveSpan("GetAllUsers");

        try
        {
            LoggingService.Log.Information("Called GetAllUsers Method");
            return Ok(await _userService.GetAllUsers());
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            return BadRequest(e.ToString());
        }
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserDto dto)
    {
        using var activity = _tracer.StartActiveSpan("UpdateUser");

        try
        {
            LoggingService.Log.Information("Called UpdateUser Method");
            return Ok(await _userService.UpdateUser(id, dto));
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            return BadRequest(e.ToString());
        }
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteUser([FromRoute] int id)
    {
        using var activity = _tracer.StartActiveSpan("DeleteUser");

        try
        {
            LoggingService.Log.Information("Called DeleteUser Method");
            return Ok(await _userService.DeleteUser(id));
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            return BadRequest(e.ToString());
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
            await _userService.RebuildDatabase();
            return Ok();
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            return BadRequest(e.ToString());
        }
    }

}