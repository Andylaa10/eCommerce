using CartService.Core.Services.DTOs;
using CartService.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CartService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    [Route("{userId}")]
    public async Task<IActionResult> GetCartByUserId([FromRoute] int userId)
    {
        try
        {
            return Ok(await _cartService.GetCartByUserId(userId));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateCart([FromBody] CreateCartDto dto)
    {
        try
        {
            return StatusCode(201, await _cartService.CreateCart(dto));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    [Route("{cartId}/products")]
    public async Task<IActionResult> AddProductToCart([FromRoute] int cartId, [FromBody] AddProductToCartDto dto)
    {
        try
        {
            return Ok(await _cartService.AddProductToCart(cartId, dto));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete]
    [Route("{cartId}/products/{productId}")]
    public async Task<IActionResult> RemoveProductFromCart([FromRoute] int cartId, [FromRoute] string productId)
    {
        try
        {
            return Ok(await _cartService.RemoveProductFromCart(cartId, productId));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    [Route("rebuild")]
    public async Task<IActionResult> RebuildDatabase()
    {
        try
        {
            await _cartService.RebuildDatabase();
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}