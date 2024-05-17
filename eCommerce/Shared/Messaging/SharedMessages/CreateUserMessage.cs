namespace Messaging.SharedMessages;

public class CreateUserMessage
{
    public string Message { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; }

    public CreateUserMessage(string message, string email, string password, DateTime createdAt)
    {
        Message = message;
        Email = email;
        Password = password;
        CreatedAt = createdAt;
    }
}