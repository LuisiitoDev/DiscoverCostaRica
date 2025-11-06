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
        var request = client.CreateInvokeMethodRequest(
            appId: "azureFunction",
            methodName: "api/LogsFunction",
            data: log);

        await client.InvokeMethodAsync(request);
    }
}
