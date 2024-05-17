using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Messaging;

public class MessageClient
{
    private ConnectionFactory _connectionFactory;
    private IConnection _connection;
    public MessageClient()
    {
        _connectionFactory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
        _connectionFactory.UserName = "guest";
        _connectionFactory.Password = "guest";
        _connection = _connectionFactory.CreateConnection();
    }

    public void Send<T>(T message, string exchangeName, string routingKey)
    {
        var channel = _connection.CreateModel();
        
        channel.ExchangeDeclare(
            exchange: exchangeName, 
            type: ExchangeType.Direct);

        var messageJson = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(messageJson);
        
        channel.BasicPublish(exchangeName, routingKey, null, body);
    }

    public void Listen<T>(Action<T> handler, string exchangeName, string queueName, string routingKey)
    {
        var channel = _connection.CreateModel();
        
        channel.ExchangeDeclare(
            exchange: exchangeName, 
            type: ExchangeType.Direct);
        
        channel.ExchangeDeclare(
            exchange: $"{exchangeName}-dlq", 
            type: ExchangeType.Fanout);
        
        channel.QueueDeclare(
            queue: queueName, 
            arguments: new Dictionary<string, object>{
                {"x-dead-letter-exchange", $"{exchangeName}-dlq"},
                {"x-message-ttl", 5000},
            });
        
        channel.QueueBind(queueName, exchangeName, routingKey);
        
        ListenMainExchange(channel, handler, queueName);
        
        channel.QueueDeclare($"{queueName}-dlq");
        channel.QueueBind($"{queueName}-dlq", $"{exchangeName}-dlq", "");
        
        ListenDLQ(channel, handler, queueName);
    }

    private void ListenMainExchange<T>(IModel channel, Action<T> handler, string queueName)
    {
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var deserializedMessage = JsonSerializer.Deserialize<T>(message);
            handler(deserializedMessage);
            channel.BasicAck(ea.DeliveryTag, false);
        };

        channel.BasicConsume(queue: queueName, consumer: consumer);
    }
    
    private void ListenDLQ<T>(IModel channel, Action<T> handler, string queueName)
    {
        // Need to do something with the dead messages here
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var deserializedMessage = JsonSerializer.Deserialize<T>(message);
            handler(deserializedMessage);
            channel.BasicAck(ea.DeliveryTag, false);
        };

        channel.BasicConsume(queue: queueName, consumer: consumer);
    }
    
    

    // private readonly IBus _bus;
    //
    // public MessageClient(IBus bus)
    // {
    //     _bus = bus;
    // }
    //
    // #region P2P
    // public async Task Send<T>(T message, string exchangeName, string routingKey)
    // {
    //     var exchange = await _bus.Advanced.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct);
    //     
    //     //Maybe this would work
    //     await _bus.Advanced.PublishAsync(exchange, routingKey, false, new Message<T>(message));
    // }
    //
    // public async Task Listen<T>(Action<T> handler, string exchangeName, string queueName, string routingKey)
    // {
    //     var mainExchange = await _bus.Advanced.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct);
    //
    //     var dlqExchangeName = $"dlq{exchangeName}";
    //
    //     var dlqExchange = await DeclareExchange(dlqExchangeName, ExchangeType.Fanout);
    //
    //     // Find some rules to determine whether the should be publish to the main queue or to the dead letter queue   
    //     var expires = new TimeSpan(0, 0, 0, 5);
    //     
    //     var mainQueue = await _bus.Advanced.QueueDeclareAsync(queueName, 
    //         x => x 
    //             .WithMessageTtl(new TimeSpan().Add(expires))
    //             .WithDeadLetterExchange(dlqExchange));
    //
    //     await _bus.Advanced.BindAsync(mainExchange, mainQueue, routingKey);
    //     await _bus.SendReceive.ReceiveAsync(queueName, handler);
    // }
    // #endregion
    //
    // public async Task ListenOnDLQ<T>(Exchange dlqExchange, string queueName, Action<T> handler)
    // {
    //     var dlqQueueName = $"dlq{queueName}";
    //     var dlqQueue = await _bus.Advanced.QueueDeclareAsync(dlqQueueName);
    //     await _bus.Advanced.BindAsync(dlqExchange, dlqQueue, string.Empty);
    //     await _bus.SendReceive.ReceiveAsync(queueName, handler);
    // }
    //
    // public async Task<Exchange> DeclareExchange(string dlqExchangeName, string exchangeType)
    // {
    //     return await _bus.Advanced.ExchangeDeclareAsync(dlqExchangeName, exchangeType);
    // }
}