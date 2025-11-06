using DiscoverCostaRica.VolcanoService.Api.Handler;

namespace DiscoverCostaRica.VolcanoService.Api.Extensions;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapVolcanoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var groups = endpoints.MapGroup("api/v1/volcanoes");

        groups.MapGet("/", VolcanoHandler.GetVolcanos);
        groups.MapGet("/{id}", VolcanoHandler.GetVolcanoById);
        groups.MapGet("/province/{provinceId}", VolcanoHandler.GetVolcanosByProvince);

        return endpoints;
    }
}
