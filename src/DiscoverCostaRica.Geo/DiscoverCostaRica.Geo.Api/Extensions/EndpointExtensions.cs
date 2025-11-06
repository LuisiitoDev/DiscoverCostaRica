using DiscoverCostaRica.Geo.Api.Handler;

namespace DiscoverCostaRica.Geo.Api.Extensions;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapGeoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var groups = endpoints.MapGroup("api/v1/geo");

        groups.MapGet("/provinces", GeoHandler.GetProvinces);
        groups.MapGet("/cantons/{provinceId}", GeoHandler.GetCantonsByProvince);
        groups.MapGet("/districts/{cantonId}", GeoHandler.GetDistrictsByCanton);

        return endpoints;
    }
}
