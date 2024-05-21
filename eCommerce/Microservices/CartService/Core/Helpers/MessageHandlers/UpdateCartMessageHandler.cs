using CartService.Core.Services.DTOs;
using CartService.Core.Services.Interfaces;
using Messaging;
using Messaging.SharedMessages;
using MonitoringService;
using OpenTelemetry.Trace;

namespace CartService.Core.Helpers.MessageHandlers;

public class UpdateCartMessageHandler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Tracer _tracer;

    public UpdateCartMessageHandler(IServiceProvider serviceProvider, Tracer tracer)
    {
        _serviceProvider = serviceProvider;
        _tracer = tracer;
    }

    public async void HandleUpdateCart(UpdateCartMessage message)
    {
        Console.WriteLine(message.Message);

        using var activity = _tracer.StartActiveSpan("HandleUpdateCart");

        // TODO Add monitoring
        // TODO Add dlq
        try
        {
            LoggingService.Log.Information("Called HandleUpdateCart Message Method");

            using var scope = _serviceProvider.CreateScope();
            var cartService = scope.ServiceProvider.GetRequiredService<ICartService>();

            var cart = await cartService.GetCartByUserId(message.UserId);

            var prices = cart.Products.Select(p => p.Price).ToList();

            float totalPrice = 0;

            if (prices.Count != 0)
            {
                totalPrice += prices.Sum();
            }
            
            var dto = new UpdateCartDto{TotalPrice = totalPrice, UserId = message.UserId};
            await cartService.UpdateCart(message.UserId, dto);
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

        var messageClient = new MessageClient();

        const string exchangeName = "UpdateCartExchange";
        const string queueName = "UpdateCartQueue";
        const string routingKey = "UpdateCart";

        messageClient.Listen<UpdateCartMessage>(HandleUpdateCart, exchangeName, queueName, routingKey);
    }
}