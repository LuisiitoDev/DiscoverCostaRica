using Aspire.Hosting;
using DiscoverCostaRica.AppHost.Tests.Constants.Services;

namespace DiscoverCostaRica.AppHost.Tests.Infrastructure;

public class Fixture<TServiceType> : IAsyncLifetime where TServiceType : IBaseServiceType, new()
{
    public Uri? Endpoint { get; private set; }
    private DistributedApplication? _app;

    public DistributedApplication App => _app ?? throw new InvalidOperationException("The fixture has not been initialized.");

    public async Task InitializeAsync()
    {
        _app = await DiscoverCostaRicaDistributedApplicationFactory
            .Create<Projects.DiscoverCostaRica_AppHost>(CancellationToken.None);

        await App.StartAsync(CancellationToken.None);

        var service = new TServiceType();
        Endpoint = App.GetEndpoint(service.Name, "https");
    }

    public async Task DisposeAsync()
    {
        if (_app is null) return;

        await _app.StopAsync(CancellationToken.None);
        await _app.DisposeAsync();
    }

    public HttpClient CreateClient()
    {
        var factory = App.Services.GetRequiredService<IHttpClientFactory>();
        var http = factory.CreateClient("IntegrationTestClient");
        http.BaseAddress = Endpoint;
        return http;
    }
}
