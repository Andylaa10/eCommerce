namespace CartService.Core.Helpers.MessageHandlers;

public class DeleteCartIfUserIsDeletedHandler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public DeleteCartIfUserIsDeletedHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}