using CartService.Core.Services.DTOs;
using CartService.Core.Services.Interfaces;
using Messaging;
using Messaging.SharedMessages;

namespace CartService.Core.Helpers.MessageHandlers;

public class CreateCartMessageHandler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public CreateCartMessageHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private async void HandleCreateCart(CreateCartMessage cart)
    {
        Console.WriteLine(cart.Message);

        // TODO Add monitoring
        // TODO Add dlq

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var cartService = scope.ServiceProvider.GetRequiredService<ICartService>();
            var dto = new CreateCartDto
            {
                UserId = cart.UserId,
                TotalPrice = cart.TotalPrice
            };

            await cartService.CreateCart(dto);
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


        const string exchangeName = "CreateCartExchange";
        const string queueName = "CreateCartQueue";
        const string routingKey = "CreateCart";

        messageClient.Listen<CreateCartMessage>(HandleCreateCart, exchangeName, queueName, routingKey);
    }
}