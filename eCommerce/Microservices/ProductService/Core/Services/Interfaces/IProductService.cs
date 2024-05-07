using ProductService.Core.Entities;
using ProductService.Core.Entities.Helper;
using ProductService.Core.Services.DTOs;

namespace ProductService.Core.Services.Interfaces;

public interface IProductService
{
    public Task<PaginatedResult<Product>> GetProducts(PaginatedDto dto);

    public Task CreateProduct(CreateProductDto product);

    public Task UpdateProduct(string id, UpdatedProductDto product);

    public Task DeleteProduct(string id);
}