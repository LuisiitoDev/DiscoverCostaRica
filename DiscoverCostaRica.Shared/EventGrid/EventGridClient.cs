using Azure;
using Azure.Messaging.EventGrid;
using Microsoft.Extensions.Options;

namespace DiscoverCostaRica.Shared.EventGrid;

public class EventGridClient(IOptionsMonitor<EventGridOptions> options) : IEventGridClient
{
    public void PublishEvent(EventGridEvent message)
    {
        var credential = new AzureKeyCredential(options.CurrentValue.TopicKey);
        var client = new EventGridPublisherClient(new Uri(options.CurrentValue.TopicEndpoint), credential);

        _ = client.SendEventAsync(message);
    }
}
