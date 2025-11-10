using DiscoverCostaRica.Shared.ApiVersioning;
using DiscoverCostaRica.Shared.Authentication;
using DiscoverCostaRica.Shared.Routes;
using DiscoverCostaRica.VolcanoService.Api.Handler;

namespace DiscoverCostaRica.VolcanoService.Api.Extensions;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapVolcanoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var volcano = endpoints.BuildEndpointGroup(1.0, RoutesConstants.Volcanoes.Group);

        volcano.Map(RoutesConstants.Volcanoes.Volcano, VolcanoHandler.GetVolcanos).RequireAuthorization(AuthConstants.Policies.VolcanoRead);
        volcano.Map(RoutesConstants.Volcanoes.ByProvince, VolcanoHandler.GetVolcanosByProvince).RequireAuthorization(AuthConstants.Policies.VolcanoRead);
        volcano.Map(RoutesConstants.Volcanoes.ById, VolcanoHandler.GetVolcanoById).RequireAuthorization(AuthConstants.Policies.VolcanoRead);

        return endpoints;
    }
}
