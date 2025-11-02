using DiscoverCostaRica.Shared.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace DiscoverCostaRica.Shared.logging;

public class RabbitMqLoggerProvider : ILoggerProvider
{
    private readonly RabbitMqLoggerOptions _options;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqLoggerProvider(IOptionsMonitor<RabbitMqLoggerOptions> options)
    {
        _options = options.CurrentValue;

        var factory = new ConnectionFactory() { HostName = options.CurrentValue.HostName };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(
            queue: options.CurrentValue.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new RabbitMqLogger(_channel, _options.QueueName, categoryName);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
