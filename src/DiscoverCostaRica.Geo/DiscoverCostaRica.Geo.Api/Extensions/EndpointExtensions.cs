using DiscoverCostaRica.Geo.Api.Handler;
using DiscoverCostaRica.Shared.ApiVersioning;
using DiscoverCostaRica.Shared.Authentication;

namespace DiscoverCostaRica.Geo.Api.Extensions;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapGeoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var versionSet = endpoints.CreateGlobalVersionSet();
        var groups = endpoints.MapGroup("/api/v{version:apiVersion}/geo")
                              .WithApiVersionSet(versionSet)
                              .MapToApiVersion(1.0);

        groups.MapGet("/provinces", GeoHandler.GetProvinces)
              .RequireAuthorization(AuthConstants.Policies.GeoRead);

        groups.MapGet("/cantons/{provinceId}", GeoHandler.GetCantonsByProvince)
              .RequireAuthorization(AuthConstants.Policies.GeoRead);

        groups.MapGet("/districts/{cantonId}", GeoHandler.GetDistrictsByCanton)
              .RequireAuthorization(AuthConstants.Policies.GeoRead);

        return endpoints;
    }
}
