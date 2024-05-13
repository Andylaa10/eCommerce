using EasyNetQ;
using EasyNetQ.Topology;

namespace Messaging;

public class MessageClient
{
    private readonly IBus _bus;

    public MessageClient(IBus bus)
    {
        _bus = bus;
    }

    #region P2P
    public async Task Send<T>(T message, string exchangeName, string routingKey)
    {
        var exchange = await _bus.Advanced.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct);
        
        //Maybe this would work
        await _bus.Advanced.PublishAsync(exchange, routingKey, false, new Message<T>(message));
    }

    public async Task Listen<T>(Action<T> handler, string exchangeName, string queueName, string routingKey)
    {
        var mainExchange = await _bus.Advanced.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct);

        var dlqExchangeName = $"dlq{exchangeName}";

        var dlqExchange = await DeclareExchange(dlqExchangeName, ExchangeType.Fanout);

        // Find some rules to determine whether the should be publish to the main queue or to the dead letter queue   
        var expires = new TimeSpan(0, 0, 0, 5);
        
        var mainQueue = await _bus.Advanced.QueueDeclareAsync(queueName, 
            x => x 
                .WithMessageTtl(new TimeSpan().Add(expires))
                .WithDeadLetterExchange(dlqExchange));

        await _bus.Advanced.BindAsync(mainExchange, mainQueue, routingKey);
        await _bus.SendReceive.ReceiveAsync(queueName, handler);
    }
    #endregion

    public async Task ListenOnDLQ<T>(Exchange dlqExchange, string queueName, Action<T> handler)
    {
        var dlqQueueName = $"dlq{queueName}";
        var dlqQueue = await _bus.Advanced.QueueDeclareAsync(dlqQueueName);
        await _bus.Advanced.BindAsync(dlqExchange, dlqQueue, string.Empty);
        await _bus.SendReceive.ReceiveAsync(queueName, handler);
    }

    public async Task<Exchange> DeclareExchange(string dlqExchangeName, string exchangeType)
    {
        return await _bus.Advanced.ExchangeDeclareAsync(dlqExchangeName, exchangeType);
    }
}