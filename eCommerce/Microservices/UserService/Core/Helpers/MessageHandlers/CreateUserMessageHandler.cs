using Messaging;
using Messaging.SharedMessages;
using MonitoringService;
using OpenTelemetry.Trace;
using UserService.Core.Services.DTOs;
using UserService.Core.Services.Interfaces;

namespace UserService.Core.Helpers.MessageHandlers;

public class CreateUserMessageHandler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Tracer _tracer;
    private readonly MessageClient _messageClient;

    public CreateUserMessageHandler(IServiceProvider serviceProvider, Tracer tracer, MessageClient messageClient)
    {
        _serviceProvider = serviceProvider;
        _tracer = tracer;
        _messageClient = messageClient;
    }

    private async void HandleCreateUser(CreateUserMessage user)
    {
        Console.WriteLine(user.Message);

        using var activity = _tracer.StartActiveSpan("HandleCreateUser");
        // TODO Add dlq logic
        try
        {
            LoggingService.Log.Information("Called HandleCreateUser Message Method");
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
            LoggingService.Log.Error(e.Message);
            Console.WriteLine(e);
            throw new ArgumentException("Something went wrong");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Message handler is running..");
        
        const string exchangeName = "CreateUserExchange";
        const string queueName = "CreateUserQueue";
        const string routingKey = "CreateUser";

        _messageClient.Listen<CreateUserMessage>(HandleCreateUser, exchangeName, queueName, routingKey);
    }
}