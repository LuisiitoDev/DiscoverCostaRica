using Aspire.Hosting;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace DiscoverCostaRica.Tests.Infraestructure;

internal static class DistributedApplicationTestFactory
{
    public static async Task<DistributedApplication> CreateAsync(CancellationToken cancellationToken)
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.DiscoverCostaRica_AppHost>(cancellationToken);

        appHost.Services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSimpleConsole();
            logging.SetMinimumLevel(LogLevel.Debug);
            logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);

        });

        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        return await appHost.BuildAsync(cancellationToken);
    }
}
