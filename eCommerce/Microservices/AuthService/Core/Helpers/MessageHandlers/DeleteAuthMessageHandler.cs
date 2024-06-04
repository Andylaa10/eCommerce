using AuthService.Core.Services.Interfaces;
using Messaging;
using Messaging.SharedMessages;
using MonitoringService;
using OpenTelemetry.Trace;

namespace AuthService.Core.Helpers.MessageHandlers;

public class DeleteAuthMessageHandler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MessageClient _messageClient;
    private readonly Tracer _tracer;

    public DeleteAuthMessageHandler(IServiceProvider serviceProvider, MessageClient messageClient, Tracer tracer)
    {
        _serviceProvider = serviceProvider;
        _messageClient = messageClient;
        _tracer = tracer;
    }

    private async void HandleDeleteAuth(DeleteAuthMessage message)
    {
        using var activity = _tracer.StartActiveSpan("HandleDeleteAuth");
        try
        {
            Console.WriteLine(message.Message);
            LoggingService.Log.Information("Called HandleDeleteAuth Message Method");
            using var scope = _serviceProvider.CreateScope();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
            await authService.DeleteAuth(message.UserId);
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            Console.WriteLine(e);
            throw;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Message handler is running..");
        
        const string exchangeName = "DeleteAuthExchange";
        const string queueName = "DeleteAuthQueue";
        const string routingKey = "DeleteAuth";

        _messageClient.Listen<DeleteAuthMessage>(HandleDeleteAuth, exchangeName, queueName, routingKey);
    }
}