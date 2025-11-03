using Azure.Messaging.EventGrid;

namespace DiscoverCostaRica.Shared.EventGrid;

public interface IEventGridClient
{
    void PublishEvent(EventGridEvent message);
}
