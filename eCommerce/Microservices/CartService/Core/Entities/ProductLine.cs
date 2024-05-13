using MongoDB.Bson;

namespace CartService.Core.Entities;

public class ProductLine
{
    public ObjectId Id { get; set; }
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public float Price { get; set; }
}