using CartService.Core.Entities;
using CartService.Core.Services.Interfaces;

namespace CartService.Core.Services;

public class CartService : ICartService
{
    public Task<Cart> CreateCart(Cart cart)
    {
        throw new NotImplementedException();
    }

    public Task<Cart> GetCartByUserId(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<Cart> AddProductToCart(int userId, Object product)
    {
        throw new NotImplementedException();
    }

    public Task<Cart> RemoveProductFromCart(int userId, int productId)
    {
        throw new NotImplementedException();
    }
    
    public Task RebuildDatabase()
    {
        throw new NotImplementedException();
    }
}