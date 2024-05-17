namespace Messaging.SharedMessages;

public class CreateCartMessage
{
    public string Message { get; set; }
    public int UserId { get; set; }
    public float TotalPrice { get; set; } = 0.0f;
}