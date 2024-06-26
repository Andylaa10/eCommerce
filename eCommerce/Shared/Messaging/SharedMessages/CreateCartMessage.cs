﻿namespace Messaging.SharedMessages;

public class CreateCartMessage
{
    public string Message { get; set; }
    public int UserId { get; set; }
    public float TotalPrice { get; set; }

    public CreateCartMessage(string message, int userId, float totalPrice)
    {
        Message = message;
        UserId = userId;
        TotalPrice = totalPrice;
    }
}