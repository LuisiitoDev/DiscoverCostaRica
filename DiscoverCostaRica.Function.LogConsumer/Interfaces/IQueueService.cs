using RabbitMQ.Client;

namespace DiscoverCostaRica.Function.LogConsumer.Interfaces;

public interface IQueueService
{
    Task ProcessQueue();
}
