using Microsoft.AspNetCore.Mvc;
using UserService.Core.Services.DTOs;
using UserService.Core.Services.Interfaces;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        try
        {
            return StatusCode(201, await _userService.AddUser(dto));
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetUserById([FromRoute] int id)
    {
        try
        {
            return Ok(await _userService.GetUserById(id));
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }

    [HttpGet]
    [Route("Users")]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            return Ok(await _userService.GetAllUsers());
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserDto dto)
    {
        try
        {
            return Ok(await _userService.UpdateUser(id, dto));
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteUser([FromRoute] int id)
    {
        try
        {
            return Ok(await _userService.DeleteUser(id));
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }

    [HttpPost]
    [Route("Rebuild")]
    public async Task<IActionResult> RebuildDatabase()
    {
        try
        {
            await _userService.RebuildDatabase();
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }
}