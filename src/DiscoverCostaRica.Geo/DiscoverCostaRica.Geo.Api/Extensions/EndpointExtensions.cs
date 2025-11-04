using DiscoverCostaRica.Geo.Api.Handler;

namespace DiscoverCostaRica.Geo.Api.Extensions;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapGeoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var groups = endpoints.MapGroup("api/v1/geo");

        groups.MapGet("/Provinces", GeoHandler.GetProvinces);
        groups.MapGet($"/Canton?provinceId={{provinceId}}", GeoHandler.GetCantonsByProvince);
        groups.MapGet($"/Districts?provinceId={{provinceId}}", GeoHandler.GetDistrictsByCanton);

        return endpoints;
    }
}
