using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DiscoverCostaRica.Tests.Infraestructure;

internal static class DistributedApplicationTestFactory
{
    /// <summary>
    /// Creates the distributed application without authentication.
    /// EntraId configuration is explicitly cleared to bypass authentication during tests.
    /// </summary>
    public static async Task<DistributedApplication> CreateAsync(CancellationToken cancellationToken)
    {
        // Set environment to Testing to load appsettings.Testing.json
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Testing");
        
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

        // Add explicit configuration to disable EntraId authentication
        var testConfig = new Dictionary<string, string?>
        {
            ["EntraId:TenantId"] = "",
            ["EntraId:ClientId"] = "",
            ["EntraId:Instance"] = "",
            ["EntraId:Audience"] = "",
            ["EntraId:Scopes"] = ""
        };

        appHost.Configuration.AddInMemoryCollection(testConfig);

        return await appHost.BuildAsync(cancellationToken);
    }
}
