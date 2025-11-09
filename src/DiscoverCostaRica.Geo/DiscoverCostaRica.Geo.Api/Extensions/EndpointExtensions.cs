using DiscoverCostaRica.Geo.Api.Handler;
using DiscoverCostaRica.Shared.ApiVersioning;
using DiscoverCostaRica.Shared.Authentication;
using DiscoverCostaRica.Shared.Routes;

namespace DiscoverCostaRica.Geo.Api.Extensions;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapGeoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var geo = endpoints.BuildEndpointGroup(1.0, RoutesConstants.Geo.Group);

        geo.MapGet(RoutesConstants.Geo.Provinces, GeoHandler.GetProvinces).RequireAuthorization(AuthConstants.Policies.GeoRead);
        geo.MapGet(RoutesConstants.Geo.Canton, GeoHandler.GetCantonsByProvince).RequireAuthorization(AuthConstants.Policies.GeoRead);
        geo.MapGet(RoutesConstants.Geo.Districts, GeoHandler.GetDistrictsByCanton).RequireAuthorization(AuthConstants.Policies.GeoRead);

        return endpoints;
    }
}
