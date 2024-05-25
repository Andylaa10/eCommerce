using Microsoft.AspNetCore.Mvc;
using MonitoringService;
using OpenTelemetry.Trace;
using ProductService.Core.Services.DTOs;
using ProductService.Core.Services.Interfaces;

namespace ProductService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly Tracer _tracer;

    public ProductController(IProductService productService, Tracer tracer)
    {
        _productService = productService;
        _tracer = tracer;
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetProductById([FromRoute] string id)
    {
        using var activity = _tracer.StartActiveSpan("GetProductById");
        
        try
        {
            LoggingService.Log.Information("Called GetProductById Method");
            return Ok(await _productService.GetProductById(id));
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            return BadRequest(e.ToString());
        }
    }
    
    [HttpGet]
    [Route("Products")]
    public async Task<IActionResult> GetProducts([FromQuery] PaginatedDto dto)
    {
        using var activity = _tracer.StartActiveSpan("GetProducts");
        
        try
        {
            LoggingService.Log.Information("Called GetProducts Method");
            return Ok(await _productService.GetProducts(dto));
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            return BadRequest(e.ToString());
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
    {
        using var activity = _tracer.StartActiveSpan("CreateProduct");

        try
        {
            LoggingService.Log.Information("Called CreateProduct Method");
            return StatusCode(201, await _productService.CreateProduct(dto));
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            return BadRequest(e.ToString());
        }
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] string id, [FromBody] UpdatedProductDto dto)
    {
        using var activity = _tracer.StartActiveSpan("UpdateProduct");

        try
        {
            LoggingService.Log.Information("Called UpdateProduct Method");
            return Ok(await _productService.UpdateProduct(id, dto));
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            return BadRequest(e.ToString());
        }
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] string id)
    {
        using var activity = _tracer.StartActiveSpan("DeleteProduct");

        try
        {
            LoggingService.Log.Information("Called DeleteProduct Method");
            return Ok(await _productService.DeleteProduct(id));
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            return BadRequest(e.ToString());
        }
    }
}