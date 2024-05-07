using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using ProductService.Core.Entities;
using ProductService.Core.Entities.Helper;
using ProductService.Core.Helpers;
using ProductService.Core.Repositories.Interfaces;

namespace ProductService.Core.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DatabaseContext _context;

    public ProductRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<Product>> GetProducts(int pageNumber, int pageSize)
    {
        var comments = await _context.Products
            .Skip(pageSize * pageNumber)
            .Take(pageSize)
            .ToListAsync();

        var totalCount = await _context.Products
            .CountAsync();

        return new PaginatedResult<Product>
        {
            Items = comments,
            TotalCount = totalCount
        };
    }

    public async Task CreateProduct(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateProduct(string id, Product updatedProduct)
    {
        var productToUpdate = await _context.Products.FirstOrDefaultAsync(p => p.Id == updatedProduct.Id);

        var objectId = new ObjectId(id);
        if (objectId != updatedProduct.Id)
        {
            throw new ArgumentException("The ids do not match");
        }

        productToUpdate.Title = updatedProduct.Title;
        productToUpdate.Description = updatedProduct.Description;
        productToUpdate.Price = updatedProduct.Price;
        productToUpdate.UpdatedAt = DateTime.Now;
        _context.Products.Update(productToUpdate);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProduct(string id)
    {
        var objectId = new ObjectId(id);
        var productToDelete = await _context.Products.FirstOrDefaultAsync(p => p.Id == objectId);
        _context.Products.Remove(productToDelete);
        await _context.SaveChangesAsync();
    }
    
}