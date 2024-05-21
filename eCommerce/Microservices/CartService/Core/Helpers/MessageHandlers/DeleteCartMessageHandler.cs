using CartService.Core.Services.Interfaces;
using Messaging;
using Messaging.SharedMessages;
using MonitoringService;
using OpenTelemetry.Trace;

namespace CartService.Core.Helpers.MessageHandlers;

public class DeleteCartMessageHandler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Tracer _tracer;
    
    public DeleteCartMessageHandler(IServiceProvider serviceProvider, Tracer tracer)
    {
        _serviceProvider = serviceProvider;
        _tracer = tracer;
    }

    public async void HandleDeleteCart(DeleteCartMessage message)
    {
        Console.WriteLine(message.Message);

        using var activity = _tracer.StartActiveSpan("HandleDeleteCart");

        // TODO Add monitoring
        // TODO Add dlq

        try
        {
            LoggingService.Log.Information("Called HandleDeleteCart Message Method");
            using var scope = _serviceProvider.CreateScope();
            var cartService = scope.ServiceProvider.GetRequiredService<ICartService>();
            
            await cartService.DeleteCart(message.UserId);
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


        const string exchangeName = "DeleteCartExchange";
        const string queueName = "DeleteCartQueue";
        const string routingKey = "DeleteCart";

        messageClient.Listen<DeleteCartMessage>(HandleDeleteCart, exchangeName, queueName, routingKey);
    }
}