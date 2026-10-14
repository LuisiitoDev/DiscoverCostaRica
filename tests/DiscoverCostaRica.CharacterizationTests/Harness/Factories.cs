using DiscoverCostaRica.Beaches.Api;
using DiscoverCostaRica.Culture.Api;
using DiscoverCostaRica.Geo.Api;
using DiscoverCostaRica.Volcano.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Http.Resilience;

namespace DiscoverCostaRica.CharacterizationTests.Harness;

public sealed class GeoApiFactory(string connectionString) : CatalogApiFactory<IGeoApiMarker>(connectionString);

public sealed class BeachesApiFactory(string connectionString) : CatalogApiFactory<IBeachesApiMarker>(connectionString);

public sealed class CultureApiFactory(string connectionString) : CatalogApiFactory<ICultureApiMarker>(connectionString);

/// <summary>Volcano with its outbound HTTP calls served by an in-process Geo service that tests can count and switch off.</summary>
public sealed class VolcanoApiFactory(string connectionString, GeoApiFactory geo) : CatalogApiFactory<IVolcanoApiMarker>(connectionString)
{
    private int _geoCalls;

    /// <summary>Requests that reached Geo, including retries.</summary>
    public int GeoCalls => Volatile.Read(ref _geoCalls);

    /// <summary>When false, Geo answers 503 to everything.</summary>
    public bool GeoAvailable { get; set; } = true;

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureAll<HttpClientFactoryOptions>(options =>
            options.HttpMessageHandlerBuilderActions.Add(handlers => handlers.PrimaryHandler = new GeoLink(this, geo.Server.CreateHandler())));

        // Keep the production resilience pipeline, but do not wait seconds between retries in tests.
        services.ConfigureAll<HttpStandardResilienceOptions>(options =>
        {
            options.Retry.MaxRetryAttempts = 1;
            options.Retry.Delay = TimeSpan.FromMilliseconds(1);
        });
    }

    private sealed class GeoLink(VolcanoApiFactory owner, HttpMessageHandler geo) : DelegatingHandler(geo)
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref owner._geoCalls);
            return owner.GeoAvailable
                ? base.SendAsync(request, cancellationToken)
                : Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable));
        }
    }
}
