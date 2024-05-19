namespace Messaging.SharedMessages;

public class UpdateCartMessage
{
    public string Message { get; set; }
    public int UserId { get; set; }
    public string Cart { get; set; }

    public UpdateCartMessage(string message, int userId,string cart)
    {
        Message = message;
        UserId = userId;
        Cart = cart;
    }
}