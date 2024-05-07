using CartService.Core.Entities;
using CartService.Core.Helpers;
using CartService.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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

    public async Task<Cart> AddProductToCart(int userId, Object product)
    {
        var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
        {
            throw new NullReferenceException();
        }

        cart.Products.Add(product);
        _context.Carts.Update(cart);
        await _context.SaveChangesAsync();

        return cart;
    }


    public async Task<Cart> RemoveProductFromCart(int userId, int productId)
    {
        var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
        {
            throw new NullReferenceException();
        }

        var product = cart.Products.FirstOrDefault(p => p.Id == productId); //TODO fix this not knowing what product is
        if (product == null)
        {
            throw new NullReferenceException();
        }

        cart.Products.Remove(product);
        _context.Carts.Update(cart);
        await _context.SaveChangesAsync();

        return cart;
    }
}