using Aspire.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DiscoverCostaRica.Tests.Infraestructure
{
    internal class DiscoverCostaRicaDistributedApplicationFactory
    {
        public static async Task<DistributedApplication> Create<TProject>(CancellationToken cancellationToken) where TProject : class
        {
            var host = await DistributedApplicationTestingBuilder.CreateAsync<TProject>(cancellationToken);
            host.Configuration.AddUserSecrets<DiscoverCostaRicaDistributedApplicationFactory>();

            // logs
            host.Services.AddLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Debug);
                logging.AddFilter(host.Environment.ApplicationName, LogLevel.Debug);
                logging.AddFilter("Aspire.", LogLevel.Debug);
            });

            host.Services.AddHttpClient("IntegrationTestClient")
                         .AddHttpMessageHandler(builder =>
                         {
                             var configuration = builder.GetRequiredService<IConfiguration>();
                             return new AzureEntraHandler(
                                 configuration["AzureAd:TenantId"]!,
                                 configuration["AzureAd:ClientId"]!,
                                 configuration["AzureAd:ClientSecret"]!,
                                 configuration["AzureAd:Scope"]!
                             );
                         });

            return await host.BuildAsync(cancellationToken);
        }
    }
}
