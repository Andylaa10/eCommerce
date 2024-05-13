using ProductService.Core.Entities;
using ProductService.Core.Entities.Helper;
using ProductService.Core.Services.DTOs;

namespace ProductService.Core.Services.Interfaces;

public interface IProductService
{
    public Task<PaginatedResult<Product>> GetProducts(PaginatedDto dto);

    public Task<Product> GetProductById(string id);
    public Task<Product> CreateProduct(CreateProductDto dto);

    public Task<Product> UpdateProduct(string id, UpdatedProductDto dto);

    public Task<Product> DeleteProduct(string id);
    public Task RebuildDatabase();

}