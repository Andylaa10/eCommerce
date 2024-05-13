using CartService.Core.Entities;
using CartService.Core.Helpers;
using CartService.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace CartService.Core.Repositories;

public class CartRepository : ICartRepository
{
    private readonly DatabaseContext _context;

    public CartRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<Cart> CreateCart(Cart cart)
    {
        await _context.Carts.AddAsync(cart);
        await _context.SaveChangesAsync();
        return cart;
    }

    public async Task<Cart> GetCartByUserId(int userId)
    {
        var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId) ?? throw
            new NullReferenceException();

        return cart;
    }

    public async Task<Cart> AddProductToCart(int cartId, ProductLine product)
    {
        var objectId = new ObjectId(cartId.ToString());

        var cart = await _context.Carts.FirstOrDefaultAsync(c => c.Id == objectId);
        if (cart == null) throw new NullReferenceException();

        cart.Products.Add(product);
        _context.Carts.Update(cart);
        await _context.SaveChangesAsync();

        return cart;
    }


    public async Task<Cart> RemoveProductFromCart(int cartId, string productId)
    {
        var objectId = new ObjectId(productId);

        var cart = await _context.Carts.FirstOrDefaultAsync(c => c.Id == objectId);
        if (cart == null) throw new NullReferenceException();

        var product = cart.Products.FirstOrDefault(p => p.ProductId == productId);
        if (product == null) throw new NullReferenceException();

        cart.Products.Remove(product);
        _context.Carts.Update(cart);
        await _context.SaveChangesAsync();

        return cart;
    }

    public async Task RebuildDatabase()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();
    }
}