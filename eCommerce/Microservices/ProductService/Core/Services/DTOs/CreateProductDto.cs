namespace ProductService.Core.Services.DTOs;

public class CreateProductDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public float Price { get; set; }
    public bool InStock { get; set; }
    public int NumberInStock { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}