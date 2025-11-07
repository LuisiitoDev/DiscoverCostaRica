using DiscoverCostaRica.Shared.ApiVersioning;
using DiscoverCostaRica.VolcanoService.Api.Handler;

namespace DiscoverCostaRica.VolcanoService.Api.Extensions;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapVolcanoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var versionSet = endpoints.CreateGlobalVersionSet();
        var groups = endpoints.MapGroup("/api/v{version:apiVersion}/volcanoes")
                              .WithApiVersionSet(versionSet)
                              .MapToApiVersion(1.0);

        groups.MapGet("/", VolcanoHandler.GetVolcanos);
        groups.MapGet("/{id}", VolcanoHandler.GetVolcanoById);
        groups.MapGet("/province/{provinceId}", VolcanoHandler.GetVolcanosByProvince);

        return endpoints;
    }
}
