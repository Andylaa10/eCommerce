using System.Runtime.InteropServices.JavaScript;
using CartService.Core.Entities;

namespace CartService.Core.Repositories.Interfaces;

public interface ICartRepository
{
    public Task<Cart> CreateCart(Cart cart);
    public Task<Cart> GetCartByUserId(int userId);
    
    public Task<Cart> AddProductToCart(int userId, Object product);
    public Task<Cart> RemoveProductFromCart(int userId, int productId);
}