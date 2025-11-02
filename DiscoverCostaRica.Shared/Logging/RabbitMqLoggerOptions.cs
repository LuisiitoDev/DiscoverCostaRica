namespace DiscoverCostaRica.Shared.logging;

public class RabbitMqLoggerOptions
{
    public required string HostName { get; set; }
    public required string QueueName { get; set; }
}
