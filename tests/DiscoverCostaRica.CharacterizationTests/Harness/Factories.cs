using DiscoverCostaRica.Beaches.Api;
using DiscoverCostaRica.Culture.Api;
using DiscoverCostaRica.Geo.Api;
using DiscoverCostaRica.Volcano.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace DiscoverCostaRica.CharacterizationTests.Harness;

public sealed class GeoApiFactory(string connectionString) : CatalogApiFactory<IGeoApiMarker>(connectionString);

public sealed class BeachesApiFactory(string connectionString) : CatalogApiFactory<IBeachesApiMarker>(connectionString);

public sealed class CultureApiFactory(string connectionString) : CatalogApiFactory<ICultureApiMarker>(connectionString);

/// <summary>Volcano with its outbound HTTP calls served by an in-process Geo service.</summary>
public sealed class VolcanoApiFactory(string connectionString, GeoApiFactory geo) : CatalogApiFactory<IVolcanoApiMarker>(connectionString)
{
    protected override void ConfigureServices(IServiceCollection services) =>
        services.ConfigureAll<HttpClientFactoryOptions>(options =>
            options.HttpMessageHandlerBuilderActions.Add(handlers => handlers.PrimaryHandler = geo.Server.CreateHandler()));
}
