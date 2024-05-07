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
            await _userService.AddUser(dto);
            return StatusCode(201, "Successfully created");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
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
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            return Ok(await _userService.GetAllUsers());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserDto dto)
    {
        try
        {
            await _userService.UpdateUser(id, dto);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
}