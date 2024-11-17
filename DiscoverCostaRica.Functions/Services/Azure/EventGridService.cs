using DiscoverCostaRica.Infraestructure.Services;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;

namespace DiscoverCostaRica.Functions.Services.Azure;

public class EventGridService
{
    public static async Task PublishEventToEventGrid<TData>(TData data, ILogger logger)
    {
        var secretManager = new SecretManagerService("", "");

        var topicCredentials = new TopicCredentials(secretManager.GetSecret("TOPIC_KEY"));
        var client = new EventGridClient(topicCredentials);

        var eventGridEvent = new EventGridEvent
        {
            Id = $"{Guid.NewGuid()}",
            Data = data,
            EventTime = DateTime.UtcNow,
            Subject = "AttractionWebScraping",
            DataVersion = "1.0"
        };

        await client.PublishEventsAsync(
            new Uri(secretManager.GetSecret("TOPIC_ENDPOINT")).Host,
            [eventGridEvent]);

        logger.LogInformation($"Event posted to Event Grid: {eventGridEvent.Id}");
    }
}
