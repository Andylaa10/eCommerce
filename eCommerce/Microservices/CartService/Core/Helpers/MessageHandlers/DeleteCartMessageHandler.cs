using CartService.Core.Services.Interfaces;
using Messaging;
using Messaging.SharedMessages;

namespace CartService.Core.Helpers.MessageHandlers;

public class DeleteCartMessageHandler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public DeleteCartMessageHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async void HandleCreateCart(DeleteCartMessage message)
    {
        Console.WriteLine(message.Message);

        // TODO Add monitoring
        // TODO Add dlq

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var cartService = scope.ServiceProvider.GetRequiredService<ICartService>();
            
            await cartService.DeleteCart(message.UserId);
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


        const string exchangeName = "DeleteCartExchange";
        const string queueName = "DeleteCartQueue";
        const string routingKey = "DeleteCart";

        messageClient.Listen<DeleteCartMessage>(HandleCreateCart, exchangeName, queueName, routingKey);
    }
}