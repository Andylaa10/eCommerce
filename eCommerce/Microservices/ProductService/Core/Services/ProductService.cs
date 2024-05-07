using AutoMapper;
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

    public ProductService(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<Product>> GetProducts(PaginatedDto dto)
    {
        return await _productRepository.GetProducts(dto.PageNumber, dto.PageSize);
    }

    public async Task CreateProduct(CreateProductDto product)
    {
        await _productRepository.CreateProduct(_mapper.Map<Product>(product));
    }

    public async Task UpdateProduct(string id, UpdatedProductDto product)
    {
        await _productRepository.UpdateProduct(id, _mapper.Map<Product>(product));
    }

    public async Task DeleteProduct(string id)
    {
        await _productRepository.DeleteProduct(id);
    }
}