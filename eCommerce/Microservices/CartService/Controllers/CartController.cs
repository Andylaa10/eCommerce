using CartService.Core.Services.DTOs;
using CartService.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MonitoringService;
using OpenTelemetry.Trace;

namespace CartService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly Tracer _tracer;
    
    public CartController(ICartService cartService, Tracer tracer)
    {
        _cartService = cartService;
        _tracer = tracer;
    }

    [HttpGet]
    [Route("{userId}")]
    public async Task<IActionResult> GetCartByUserId([FromRoute] int userId)
    {
        using var activity = _tracer.StartActiveSpan("GetCartByUserId");

        try
        {
            LoggingService.Log.Information("Called GetCartByUserId Method");
            return Ok(await _cartService.GetCartByUserId(userId));
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            return BadRequest(e.ToString());
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateCart([FromBody] CreateCartDto dto)
    {
        using var activity = _tracer.StartActiveSpan("CreateCart");
        
        try
        {
            LoggingService.Log.Information("Called CreateCart Method");
            return StatusCode(201, await _cartService.CreateCart(dto));
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            return BadRequest(e.ToString());
        }
    }

    [HttpPost]
    [Route("{userId}/products")]
    public async Task<IActionResult> AddProductToCart([FromRoute] int userId, [FromBody] AddProductToCartDto dto)
    {
        using var activity = _tracer.StartActiveSpan("AddProductToCart");

        try
        {
            LoggingService.Log.Information("Called AddProductToCart Method");
            return Ok(await _cartService.AddProductToCart(userId, dto));
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            return BadRequest(e.ToString());
        }
    }

    [HttpDelete]
    [Route("{cartId}/products/{productId}")]
    public async Task<IActionResult> RemoveProductFromCart([FromRoute] int userId, [FromRoute] string productId)
    {
        using var activity = _tracer.StartActiveSpan("RemoveProductFromCart");

        try
        {
            LoggingService.Log.Information("Called RemoveProductFromCart Method");
            return Ok(await _cartService.RemoveProductFromCart(userId, productId));
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            return BadRequest(e.ToString());
        }
    }
}