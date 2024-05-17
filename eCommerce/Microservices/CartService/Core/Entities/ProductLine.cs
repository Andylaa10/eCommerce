using MongoDB.Bson;

namespace CartService.Core.Entities;

public class ProductLine
{ 
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public float Price { get; set; }
}