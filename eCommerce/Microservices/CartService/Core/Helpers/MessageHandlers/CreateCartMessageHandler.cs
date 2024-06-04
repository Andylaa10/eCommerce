using CartService.Core.Services.DTOs;
using CartService.Core.Services.Interfaces;
using Messaging;
using Messaging.SharedMessages;
using MonitoringService;
using OpenTelemetry.Trace;

namespace CartService.Core.Helpers.MessageHandlers;

public class CreateCartMessageHandler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Tracer _tracer;
    private readonly MessageClient _messageClient;

    public CreateCartMessageHandler(IServiceProvider serviceProvider, Tracer tracer, MessageClient messageClient)
    {
        _serviceProvider = serviceProvider;
        _tracer = tracer;
        _messageClient = messageClient;
    }

    private async void HandleCreateCart(CreateCartMessage cart)
    {
        using var activity = _tracer.StartActiveSpan("HandleCreateCart");
        try
        {
            Console.WriteLine(cart.Message);
            LoggingService.Log.Information("Called HandleCreateCart Message Method");
            using var scope = _serviceProvider.CreateScope();
            var cartService = scope.ServiceProvider.GetRequiredService<ICartService>();
            var dto = new CreateCartDto
            {
                UserId = cart.UserId,
                TotalPrice = cart.TotalPrice,
            };

            await cartService.CreateCart(dto);
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
        
        const string exchangeName = "CreateCartExchange";
        const string queueName = "CreateCartQueue";
        const string routingKey = "CreateCart";

        _messageClient.Listen<CreateCartMessage>(HandleCreateCart, exchangeName, queueName, routingKey);
    }
}