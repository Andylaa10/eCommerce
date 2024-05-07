using CartService.Core.Entities;

namespace CartService.Core.Services.Interfaces;

public interface ICartService
{
    
    public Task<Cart> CreateCart(Cart cart);
    public Task<Cart> GetCartByUserId(int userId);
    
    public Task<Cart> AddProductToCart(int userId, Object product);
    public Task<Cart> RemoveProductFromCart(int userId, int productId);
    public Task RebuildDatabase();
}