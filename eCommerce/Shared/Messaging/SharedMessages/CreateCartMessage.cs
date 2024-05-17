namespace Messaging.SharedMessages;

public class CreateCartMessage
{
    public string Message { get; set; }
    public int UserId { get; set; }
    public float TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<object> Products { get; } = new ();
}