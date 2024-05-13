using CartService.Core.Entities;

namespace CartService.Core.Services.DTOs;

public class CreateCartDto
{
    public int UserId { get; set; }
    public float TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ProductLine> Products { get; } = new ();
}