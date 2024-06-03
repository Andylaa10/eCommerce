using AutoMapper;
using Cache;
using DnsClient;
using MonitoringService;
using OpenTelemetry.Trace;
using Polly;
using ProductService.Core.Entities;
using ProductService.Core.Entities.Helper;
using ProductService.Core.Repositories.Interfaces;
using ProductService.Core.Services.DTOs;
using ProductService.Core.Services.Interfaces;

namespace ProductService.Core.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IRedisClient _redisClient;
    private readonly Tracer _tracer;


    public ProductService(IProductRepository productRepository, IMapper mapper, IRedisClient redisClient, Tracer tracer)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _redisClient = redisClient;
        _redisClient.Connect();
        _tracer = tracer;
    }

    public async Task<PaginatedResult<Product>> GetProducts(PaginatedDto dto)
    {
        _tracer.StartActiveSpan("GetProducts");
        LoggingService.Log.Information("Called GetProducts");
        
        // Circuit Breaker Policy  
        var circuitBreakerPolicy = Policy<PaginatedResult<Product>>
            .Handle<Exception>()
            .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30),
                (ex, duration) =>
                {
                    Console.WriteLine($"Circuit breaker opened for {duration.TotalSeconds} seconds.");
                }, () => { Console.WriteLine("Circuit breaker closed."); });

        //Fallback Policy  
        var fallbackPolicy = Policy<PaginatedResult<Product>>
            .Handle<Exception>()
            .FallbackAsync((context) =>
            {
                _tracer.StartActiveSpan("GetProductsFallback"); // If we enter fallback, start a trace
                
                var fallbackProductList = new PaginatedResult<Product>();
                fallbackProductList.Items = new List<Product>();
                fallbackProductList.TotalCount = 0;
                fallbackProductList.fallbackMessage = "Unable to retrieve products right now. Please try again later.";
                LoggingService.Log.Warning("Called Fallback Method in ProductService "); 
                return Task.FromResult(fallbackProductList);
            });


        return await fallbackPolicy.WrapAsync(circuitBreakerPolicy)
            .ExecuteAsync(async () =>
            {
                return await _productRepository.GetProducts(dto.PageNumber, dto.PageSize);
            });
    }

    public async Task<Product> GetProductById(string id)
    {
        _tracer.StartActiveSpan("GetProductById");
        LoggingService.Log.Information("GetProductById called with id: " + id); 

        
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("Id cannot be null or empty");

        var productJson = await _redisClient.GetValue($"Product:{id}");

        if (!string.IsNullOrEmpty(productJson))
            return await Task.FromResult(_redisClient.DeserializeObject<Product>(productJson)!);

        var product = await _productRepository.GetProductById(id);
        
        if (product == null)
            throw new KeyNotFoundException($"No product with id of {id}");
        
        return product;
    }

    public async Task<Product> CreateProduct(CreateProductDto dto)
    {
        _tracer.StartActiveSpan("CreateProduct");
        LoggingService.Log.Information("CreateProduct called with name: " + dto);
        
        var product = await _productRepository.CreateProduct(_mapper.Map<Product>(dto));

        var productJson = _redisClient.SerializeObject(product);
        await _redisClient.StoreValue($"Product:{product.Id}", productJson);

        return product;
    }

    public async Task<Product> UpdateProduct(string id, UpdatedProductDto dto)
    {
        _tracer.StartActiveSpan("UpdateProduct");
        LoggingService.Log.Information("UpdateProduct called with id: " + id);
        
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("Id cannot be null or empty");

        if (id != dto.Id)
            throw new ArgumentException("Id in the route does not match the id of the product");


        var product = await _productRepository.UpdateProduct(id, _mapper.Map<Product>(dto));

        var productJson = _redisClient.SerializeObject(product);
        await _redisClient.StoreValue($"Product:{product.Id}", productJson);

        return product;
    }

    public async Task<Product> DeleteProduct(string id)
    {
        _tracer.StartActiveSpan("DeleteProduct");
        LoggingService.Log.Information("DeleteProduct called with id: " + id);
        
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("Id cannot be null or empty");

        
        var product = await _productRepository.DeleteProduct(id);

        if (product is null)
            throw new KeyNotFoundException($"No product with id of {id}");
        
        await _redisClient.RemoveValue($"Product:{product.Id}");
        return product;
    }
}