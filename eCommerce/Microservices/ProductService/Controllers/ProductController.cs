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
    public async Task<IActionResult> GetProducts([FromQuery] PaginatedDto dto)
    {
        try
        {
            return Ok(await _productService.GetProducts(dto));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
    {
        try
        {
            await _productService.CreateProduct(dto);
            return StatusCode(201, "Product successfully created");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] string id, [FromBody] UpdatedProductDto dto)
    {
        try
        {
            await _productService.UpdateProduct(id, dto);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteComment([FromRoute] string id)
    {
        try
        {
            await _productService.DeleteProduct(id);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}