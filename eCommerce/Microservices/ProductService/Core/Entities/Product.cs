using MongoDB.Bson;

namespace ProductService.Core.Entities;

public class Product
{
    public ObjectId Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public float Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}