using DiscoverCostaRica.BuildingBlocks.Web.Authentication;
using DiscoverCostaRica.BuildingBlocks.Web.Versioning;

namespace DiscoverCostaRica.Geo.Api.Endpoints;

public static class GeoEndpoints
{
    /// <summary>Maps every Geo v1 route under <c>/api/v1/geo</c>; all of them need the <c>Geo.Read</c> app role.</summary>
    public static IEndpointRouteBuilder MapGeoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var geo = endpoints.BuildEndpointGroup(1.0, GeoRoutes.Group)
            .RequireAuthorization(AuthConstants.Policies.GeoRead);

        geo.MapProvinceEndpoints();
        geo.MapCantonEndpoints();
        geo.MapDistrictEndpoints();
        geo.MapLocationEndpoints();

        return endpoints;
    }
}
