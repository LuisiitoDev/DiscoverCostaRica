using DiscoverCostaRica.Shared.EventGrid;
using DiscoverCostaRica.Shared.Logging;
using Microsoft.Extensions.Logging;

namespace DiscoverCostaRica.Shared.logging;

public class EventGridLoggerProvider(IEventGridClient client) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new EventGridLogger(client, categoryName);
    }

    public void Dispose() { }
}
