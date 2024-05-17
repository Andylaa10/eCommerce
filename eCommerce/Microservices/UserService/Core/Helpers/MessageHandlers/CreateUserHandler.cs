﻿using Messaging;
using Messaging.SharedMessages;
using UserService.Core.Services.DTOs;
using UserService.Core.Services.Interfaces;

namespace UserService.Core.Helpers.MessageHandlers;

public class CreateUserHandler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public CreateUserHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private async void HandleCreateUser(CreateUserMessage user)
    {
        Console.WriteLine(user.Message);

        // TODO Add monitoring
        // TODO Add dlq
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            var dto = new CreateUserDto
            {
                Email = user.Email,
                Password = user.Password,
                CreatedAt = user.CreatedAt
            };

            await userService.AddUser(dto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new ArgumentException("Something went wrong");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Message handler is running..");

        var messageClient = new MessageClient();

        const string exchangeName = "CreateUserExchange";
        const string queueName = "CreateUserQueue";
        const string routingKey = "CreateUser";

        messageClient.Listen<CreateUserMessage>(HandleCreateUser, exchangeName, queueName, routingKey);

        // var dlqExchange = await messageClient.DeclareExchange("TestExchange", ExchangeType.Fanout);
        // const string dlqQueue = "testQueue";
        // await messageClient.ListenOnDLQ<CreateUserMessage>(dlqExchange, dlqQueue, HandleExchange);
    }
}