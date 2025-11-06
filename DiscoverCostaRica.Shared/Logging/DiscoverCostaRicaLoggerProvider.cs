using DiscoverCostaRica.Shared.Interfaces;
using DiscoverCostaRica.Shared.Logging;
using Microsoft.Extensions.Logging;

namespace DiscoverCostaRica.Shared.logging;

public class DiscoverCostaRicaLoggerProvider(ILoggerFunction logger) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new DiscoverCostaRicaLogger(logger, categoryName);
    }

    public void Dispose() { }
}
