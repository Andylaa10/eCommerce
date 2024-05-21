namespace Messaging.SharedMessages;

public class DeleteAuthMessage
{
    public string Message { get; set; }
    public int UserId { get; set; }

    public DeleteAuthMessage(string message, int userId)
    {
        Message = message;
        UserId = userId;
    }
}