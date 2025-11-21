
using Aspire.Hosting;
using DiscoverCostaRica.Tests.Constants.Services;

namespace DiscoverCostaRica.Tests.Infraestructure;

public class Fixture<TServiceType> : IAsyncLifetime where TServiceType : IBaseServiceType, new()
{
    public Uri? Endpoint { get; private set; }
    public DistributedApplication? App { get; private set; }

    public async Task InitializeAsync()
    {
        App = await DiscoverCostaRicaDistributedApplicationFactory
            .Create<Projects.DiscoverCostaRica_AppHost>(CancellationToken.None);

        await App.StartAsync(CancellationToken.None);

        var service = new TServiceType();
        Endpoint = App.GetEndpoint(service.Name, "https");
    }

    public async Task DisposeAsync()
    {
        await App.StopAsync(CancellationToken.None);
        App.Dispose();
    }

    public HttpClient CreateClient()
    {
        var factory = App.Services.GetRequiredService<IHttpClientFactory>();
        var http = factory.CreateClient("IntegrationTestClient");
        http.BaseAddress = Endpoint;
        return http;
    }
}
