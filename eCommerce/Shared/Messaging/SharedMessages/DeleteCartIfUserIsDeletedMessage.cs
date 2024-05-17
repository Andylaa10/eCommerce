namespace Messaging.SharedMessages;

public class DeleteCartIfUserIsDeletedMessage
{
    public string Message { get; set; }
    public int UserId { get; set; }

    public DeleteCartIfUserIsDeletedMessage(string message, int userId)
    {
        Message = message;
        UserId = userId;
    }
}