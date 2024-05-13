using CartService.Core.Entities;

namespace CartService.Core.Repositories.Interfaces;

public interface ICartRepository
{
    public Task<Cart> CreateCart(Cart cart);
    public Task<Cart> GetCartByUserId(int userId);

    public Task<Cart> AddProductToCart(int cartId, ProductLine product);
    public Task<Cart> RemoveProductFromCart(int cartId, string productId);

    public Task RebuildDatabase();
}