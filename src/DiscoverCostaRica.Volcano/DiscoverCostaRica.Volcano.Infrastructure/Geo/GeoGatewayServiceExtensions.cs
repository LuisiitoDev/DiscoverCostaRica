using DiscoverCostaRica.BuildingBlocks.Infrastructure.Authentication;
using DiscoverCostaRica.Contracts.Geo;
using DiscoverCostaRica.Volcano.Application.Ports;
using DiscoverCostaRica.Volcano.Infrastructure.Geo;

namespace Microsoft.Extensions.DependencyInjection;

public static class GeoGatewayServiceExtensions
{
    /// <summary>
    /// Typed HTTP client for Geo, found through service discovery and authenticated with the service's
    /// client-credentials token. Resilience (retry, circuit breaker, timeouts) comes from ServiceDefaults.
    /// </summary>
    public static IServiceCollection AddGeoLocationGateway(this IServiceCollection services)
    {
        services.AddHttpClient<IGeoLocationGateway, GeoLocationGateway>(http =>
                http.BaseAddress = new Uri($"https://{GeoContract.ServiceName}"))
            .AddHttpMessageHandler<BearerTokenHandler>();

        return services;
    }
}
