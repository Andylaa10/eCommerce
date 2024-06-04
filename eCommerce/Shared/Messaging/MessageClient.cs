using System.Text;
using System.Text.Json;
using MonitoringService;
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
            exclusive: false,
            durable: true,
            autoDelete: false,
            arguments: new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", $"{exchangeName}-dlq" },
                { "x-message-ttl", 1000 },
            });

        channel.QueueBind(queueName, exchangeName, routingKey);
        
        ListenMainExchange(channel, handler, queueName);

        channel.QueueDeclare(
            queue: $"{queueName}-dlq",
            exclusive: false,
            durable: true,
            autoDelete: false,
            arguments: new Dictionary<string, object>()
        );
        channel.QueueBind($"{queueName}-dlq", $"{exchangeName}-dlq", "");

        ListenDLQ(channel, queueName);
    }

    private void ListenMainExchange<T>(IModel channel, Action<T> handler, string queueName)
    {
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            LoggingService.Log.Information($"Message was sent to the {queueName}");
            channel.BasicNack(ea.DeliveryTag, false, false);

            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var deserializedMessage = JsonSerializer.Deserialize<T>(message);
            handler(deserializedMessage);
        };
        channel.BasicConsume(queue: queueName, consumer: consumer);
    }

    private void ListenDLQ(IModel channel, string queueName)
    {
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"DLX - Recieved new message: {message}");
            LoggingService.Log.Warning($"WARNING!!! Message was sent to the {queueName}");
            channel.BasicAck(ea.DeliveryTag, false);
        };
        channel.BasicConsume(queue: queueName, consumer: consumer);
    }
}