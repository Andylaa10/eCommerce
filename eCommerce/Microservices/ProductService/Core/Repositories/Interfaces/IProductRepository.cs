using ProductService.Core.Entities;
using ProductService.Core.Entities.Helper;

namespace ProductService.Core.Repositories.Interfaces;

public interface IProductRepository
{
    public Task<PaginatedResult<Product>> GetProducts(int pageNumber, int pageSize);
    public Task<Product> GetProductById(string id);
    public Task<Product>  CreateProduct(Product product);
    public Task<Product>  UpdateProduct(string id, Product product);
    public Task<Product>  DeleteProduct(string id);
    public Task RebuildDatabase();
}