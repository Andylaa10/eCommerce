namespace Messaging.SharedMessages;

public class UpdateCartMessage
{
    public string Message { get; set; }
    public int UserId { get; set; }
    public UpdateCartMessage(string message, int userId)
    {
        Message = message;
        UserId = userId;
    }
}