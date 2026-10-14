using DiscoverCostaRica.AppHost.Constants;

namespace DiscoverCostaRica.AppHost.Gateway;

/// <param name="Path">Public path pattern on the gateway.</param>
/// <param name="Service">Upstream service (Aspire resource name).</param>
/// <param name="Upstream">Path pattern on the service; route values from <paramref name="Path"/> fill it in.</param>
public sealed record GatewayRoute(string Path, string Service, string Upstream);

/// <summary>
/// The public route table (contracts/public-api-v1.md § Gateway). Every route is explicit; there is no
/// catch-all, so an unknown path is a 404 at the gateway (PLT-D1). Kept as data so tests can check it.
/// </summary>
public static class GatewayRoutes
{
    public static IReadOnlyList<GatewayRoute> All { get; } =
    [
        new("/provinces/{**rest}", Microservices.Geo, "/api/v1/geo/provinces/{**rest}"),
        new("/cantons/{**rest}", Microservices.Geo, "/api/v1/geo/cantons/{**rest}"),
        new("/canton/{**rest}", Microservices.Geo, "/api/v1/geo/cantons/{**rest}"), // legacy alias
        new("/districts/{**rest}", Microservices.Geo, "/api/v1/geo/districts/{**rest}"),
        new("/beaches/{**rest}", Microservices.Beaches, "/api/v1/beaches/{**rest}"), // BCH-D1
        new("/tradition/{**rest}", Microservices.Culture, "/api/v1/traditions/tradition/{**rest}"),
        new("/dish/{**rest}", Microservices.Culture, "/api/v1/traditions/dish/{**rest}"),
        new("/volcanoes/{**rest}", Microservices.Volcano, "/api/v1/volcanoes/{**rest}"),
        new("/volcano/{**rest}", Microservices.Volcano, "/api/v1/volcanoes/{**rest}"), // legacy alias, VOL-D7
        new("/province/{**rest}", Microservices.Volcano, "/api/v1/volcanoes/province/{**rest}"), // legacy alias
    ];
}
