﻿namespace Messaging.SharedMessages;

public class CreateUserMessage
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; }
}