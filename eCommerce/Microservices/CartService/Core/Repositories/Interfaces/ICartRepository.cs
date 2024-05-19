using CartService.Core.Entities;

namespace CartService.Core.Repositories.Interfaces;

public interface ICartRepository
{
    public Task<Cart> CreateCart(Cart cart);
    public Task<Cart> GetCartByUserId(int userId);
    public Task<Cart> UpdateCart(int userId, Cart cart);
    public Task<Cart> AddProductToCart(int userId, ProductLine product);
    public Task<Cart> RemoveProductFromCart(int userId, string productId);
    public Task<Cart> DeleteCart(int userId);
}