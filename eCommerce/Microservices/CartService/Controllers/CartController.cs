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
            return BadRequest(e.ToString());
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
            return BadRequest(e.ToString());
        }
    }

    [HttpPost]
    [Route("{cartId}/products")]
    public async Task<IActionResult> AddProductToCart([FromRoute] int userId, [FromBody] AddProductToCartDto dto)
    {
        try
        {
            return Ok(await _cartService.AddProductToCart(userId, dto));
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }

    [HttpDelete]
    [Route("{cartId}/products/{productId}")]
    public async Task<IActionResult> RemoveProductFromCart([FromRoute] int userId, [FromRoute] string productId)
    {
        try
        {
            return Ok(await _cartService.RemoveProductFromCart(userId, productId));
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }
}