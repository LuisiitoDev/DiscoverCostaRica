using DiscoverCostaRica.Function.LogConsumer.Interfaces;
using Microsoft.Extensions.Logging;

namespace DiscoverCostaRica.Function.LogConsumer.Logging;

public class MongoLoggerProvider(IMongoService mongo) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new MongoLogger(mongo);
    }

    public void Dispose() { }
}
