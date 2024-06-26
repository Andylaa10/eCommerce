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

    public async Task<Cart> UpdateCart(int userId, Cart dto)
    {
        var cart = await GetCartByUserId(userId);
        if (cart == null) throw new NullReferenceException();

        cart.TotalPrice = dto.TotalPrice;
        cart.UpdatedAt = DateTime.Now;

        _context.Carts.Update(cart);
        await _context.SaveChangesAsync();
        
        return cart;
    }

    public async Task<Cart> AddProductToCart(int userId, ProductLine product)
    {
        var cart = await GetCartByUserId(userId);
        if (cart == null) throw new NullReferenceException();

        cart.Products.Add(product);
        cart.UpdatedAt = DateTime.Now;
        _context.Carts.Update(cart);
        await _context.SaveChangesAsync();

        return cart;
    }


    public async Task<Cart> RemoveProductFromCart(int userId, string productId)
    {
        var cart = await GetCartByUserId(userId);
        if (cart == null) throw new NullReferenceException();

        var product = cart.Products.FirstOrDefault(p => p.ProductId == productId);
        if (product == null) throw new NullReferenceException();
        
        cart.Products.Remove(product);
        cart.UpdatedAt = DateTime.Now;
        _context.Carts.Update(cart);
        await _context.SaveChangesAsync();

        return cart;
    }

    public async Task<Cart> DeleteCart(int userId)
    {
        var cart = await GetCartByUserId(userId);
        if (cart == null) throw new NullReferenceException();

        _context.Carts.Remove(cart);
        await _context.SaveChangesAsync();

        return cart;
    }
}