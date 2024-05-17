using CartService.Core.Entities;

namespace CartService.Core.Services.DTOs;

public class CreateCartDto
{
    public int UserId { get; set; }
    public float TotalPrice { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public List<ProductLine>? Products { get; } = new ();
}