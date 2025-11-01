using DiscoverCostaRica.VolcanoService.Api.Handler;

namespace DiscoverCostaRica.VolcanoService.Api.Extensions;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapVolcanoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var groups = endpoints.MapGroup("api/v1/volcano");

        groups.MapGet("/", VolcanoHandler.GetVolcanos);
        groups.MapGet($"/Volcano?id={{id}}", VolcanoHandler.GetVolcanoById);
        groups.MapGet($"/Volcano/Province?id={{id}}", VolcanoHandler.GetVolcanosByProvince);

        return endpoints;
    }
}
