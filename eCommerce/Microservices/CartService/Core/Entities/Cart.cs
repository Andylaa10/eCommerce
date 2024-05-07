using MongoDB.Bson;

namespace CartService.Core.Entities;

public class Cart
{
    public ObjectId Id { get; set; }
    public int UserId { get; set; }
    public float TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<Object> Products { get; set; } = new ();
}