using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DiscoverCostaRica.Tests.Infraestructure;

internal static class DistributedApplicationTestFactory
{
    /// <summary>
    /// Creates the distributed application with test-specific configuration.
    /// 
    /// IMPORTANT: To properly use mock authentication (Option 1 - TestAuthenticationHandler):
    /// Your API services need to check for Testing environment and use TestAuthenticationHandler
    /// instead of Entra ID authentication. This can be done by adding this to Program.cs:
    /// 
    /// if (builder.Environment.IsEnvironment("Testing"))
    /// {
    ///     builder.Services.AddAuthentication(TestAuthenticationHandler.AuthenticationScheme)
    ///         .AddScheme&lt;AuthenticationSchemeOptions, TestAuthenticationHandler&gt;(
    ///             TestAuthenticationHandler.AuthenticationScheme, _ => { });
    /// }
    /// else
    /// {
    ///     builder.AddEntraIdAuthentication();
    /// }
    /// 
    /// CURRENT IMPLEMENTATION: Without modifying API code, tests use empty EntraId configuration
    /// which causes services to skip authentication setup (existing behavior in Extensions.cs).
    /// </summary>
    public static async Task<DistributedApplication> CreateAsync(CancellationToken cancellationToken)
    {
        // Set environment to Testing
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

        // Configure for testing: Empty EntraId config causes services to skip auth setup
        // To use proper mock authentication, uncomment this and modify API Program.cs files
        var testConfig = new Dictionary<string, string?>
        {
            ["EntraId:TenantId"] = "",
            ["EntraId:ClientId"] = "",
            ["EntraId:Instance"] = "",
            ["EntraId:Audience"] = "",
            ["EntraId:Scopes"] = "",
            
            // Marker for services that want to opt-in to test authentication
            ["Testing:UseTestAuthentication"] = "true"
        };

        appHost.Configuration.AddInMemoryCollection(testConfig);

        return await appHost.BuildAsync(cancellationToken);
    }
}

