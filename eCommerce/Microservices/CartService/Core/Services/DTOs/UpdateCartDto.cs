namespace CartService.Core.Services.DTOs;

public class UpdateCartDto
{
    public int UserId { get; set; }
    public float TotalPrice { get; set; }
    public DateTime? UpdatedAt { get; set; } = DateTime.Now;}