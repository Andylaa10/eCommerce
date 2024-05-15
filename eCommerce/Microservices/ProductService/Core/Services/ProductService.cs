using AutoMapper;
using Cache;
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
    private readonly RedisClient _redisClient;


    public ProductService(IProductRepository productRepository, IMapper mapper, RedisClient redisClient)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _redisClient = redisClient;
        //_redisClient.Connect();
    }

    public async Task<PaginatedResult<Product>> GetProducts(PaginatedDto dto)
    {
        return await _productRepository.GetProducts(dto.PageNumber, dto.PageSize);
    }

    public async Task<Product> GetProductById(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("Id cannot be null or empty");

        var productJson = _redisClient.GetValue($"Product:{id}");

        if (!string.IsNullOrEmpty(productJson))
            return await Task.FromResult(_redisClient.DeserializeObject<Product>(productJson)!);

        return await _productRepository.GetProductById(id);
    }

    public async Task<Product> CreateProduct(CreateProductDto dto)
    {
        var product = await _productRepository.CreateProduct(_mapper.Map<Product>(dto));

        var productJson = _redisClient.SerializeObject(product);
        _redisClient.StoreValue($"Product:{product.Id}", productJson);

        return product;
    }

    public async Task<Product> UpdateProduct(string id, UpdatedProductDto dto)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("Id cannot be null or empty");

        if (id != dto.Id)
            throw new ArgumentException("Id in the route does not match the id of the product");
            
            
        var product = await _productRepository.UpdateProduct(id, _mapper.Map<Product>(dto));
        
        var productJson = _redisClient.SerializeObject(product);
        _redisClient.StoreValue($"Product:{product.Id}", productJson);

        return product;
    }

    public async Task<Product> DeleteProduct(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("Id cannot be null or empty");
        
        var product = await _productRepository.DeleteProduct(id);
        _redisClient.RemoveValue($"Product:{product.Id}");

        return product;
    }
}