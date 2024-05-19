using AuthService.Core.Services.Interfaces;
using Messaging;
using Messaging.SharedMessages;

namespace AuthService.Core.Helpers.MessageHandlers;

public class DeleteAuthMessageHandler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public DeleteAuthMessageHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private async void HandleDeleteAuth(DeleteAuthMessage message)
    {
        // TODO Add monitoring
        // TODO Add dlq
        try
        {
            Console.WriteLine(message.Message);
            using var scope = _serviceProvider.CreateScope();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
            await authService.DeleteAuth(message.UserId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Message handler is running..");

        var messageClient = new MessageClient();

        const string exchangeName = "DeleteAuthExchange";
        const string queueName = "DeleteAuthQueue";
        const string routingKey = "DeleteAuth";

        messageClient.Listen<DeleteAuthMessage>(HandleDeleteAuth, exchangeName, queueName, routingKey);
    }
}