using DiscoverCostaRica.Function.LogConsumer.Interfaces;
using DiscoverCostaRica.Function.LogConsumer.Models;
using RabbitMQ.Client;
using System.Text.Json;

namespace DiscoverCostaRica.Function.LogConsumer.Services;

public class QueueService : IQueueService
{
    private readonly IMongoService _mongo;
    private readonly ConnectionFactory _factory;
    public QueueService(IMongoService mongo)
    {
        _mongo = mongo;
        _factory = new ConnectionFactory()
        {
            HostName = Environment.GetEnvironmentVariable("RabbitMqHost")!,
            UserName = Environment.GetEnvironmentVariable("RabbitMqUser")!,
            Password = Environment.GetEnvironmentVariable("RabbitMqPassword")!,
            Port = 5672,
        };
    }

    public async Task ProcessQueue()
    {
        using var connection = await _factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync("logs", false, false, false, null);

        var result = await channel.BasicGetAsync("logs", false);

        if(result is not null)
        {
            var body = result.Body.ToArray();
            var log = JsonSerializer.Deserialize<LogModel>(body);
            await _mongo.Log(log!);
        }
    }
}
