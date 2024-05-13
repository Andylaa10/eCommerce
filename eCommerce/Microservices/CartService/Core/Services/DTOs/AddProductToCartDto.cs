namespace CartService.Core.Services.DTOs;

public class AddProductToCartDto
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public float Price { get; set; }
}