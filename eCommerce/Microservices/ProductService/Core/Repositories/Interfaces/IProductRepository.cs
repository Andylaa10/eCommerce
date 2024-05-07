using ProductService.Core.Entities;
using ProductService.Core.Entities.Helper;
using ProductService.Core.Services.DTOs;

namespace ProductService.Core.Repositories.Interfaces;

public interface IProductRepository
{
    public Task<PaginatedResult<Product>> GetProducts(int pageNumber, int pageSize);

    public Task CreateProduct(Product product);

    public Task UpdateProduct(string id, Product product);

    public Task DeleteProduct(string id);
}