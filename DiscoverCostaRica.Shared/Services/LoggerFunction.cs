using Dapr.Client;
using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Shared.Interfaces;
using DiscoverCostaRica.Shared.Logging;

namespace DiscoverCostaRica.Shared.Services;

[SingletonService]
public class LoggerFunction(DaprClient client) : ILoggerFunction
{
    public async Task SendLogger(LogEntryModel log)
    {
        await client.SaveStateAsync("mongo-logs", Guid.CreateVersion7().ToString(), log);
    }
}
