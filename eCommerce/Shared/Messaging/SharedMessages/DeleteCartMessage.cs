namespace Messaging.SharedMessages;

public class DeleteCartMessage
{
    public string Message { get; set; }
    public int UserId { get; set; }

    public DeleteCartMessage(string message, int userId)
    {
        Message = message;
        UserId = userId;
    }
}