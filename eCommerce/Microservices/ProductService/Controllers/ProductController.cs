using Microsoft.AspNetCore.Mvc;
using ProductService.Core.Services.DTOs;
using ProductService.Core.Services.Interfaces;

namespace ProductService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetProductById([FromRoute] string id)
    {
        try
        {
            return Ok(await _productService.GetProductById(id));
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }
    
    [HttpGet]
    [Route("Products")]
    public async Task<IActionResult> GetProducts([FromQuery] PaginatedDto dto)
    {
        try
        {
            return Ok(await _productService.GetProducts(dto));
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
    {
        try
        {
            return StatusCode(201, await _productService.CreateProduct(dto));
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] string id, [FromBody] UpdatedProductDto dto)
    {
        try
        {
            return Ok(await _productService.UpdateProduct(id, dto));
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] string id)
    {
        try
        {
            return Ok(await _productService.DeleteProduct(id));
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }
}