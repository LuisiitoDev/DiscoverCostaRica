using DiscoverCostaRica.Culture.Api.Handlers;
using DiscoverCostaRica.Shared.ApiVersioning;
using DiscoverCostaRica.Shared.Authentication;
using DiscoverCostaRica.Shared.Routes;

namespace DiscoverCostaRica.Culture.Api.Extensions;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapCultureEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var traditions = endpoints.BuildEndpointGroup(1.0, RoutesConstants.Traditions.Group);

        traditions.MapGet(RoutesConstants.Traditions.Tradition, CultureHandler.TraditionHandler.GetTraditions).RequireAuthorization(AuthConstants.Policies.CultureRead);
        traditions.MapGet(RoutesConstants.Traditions.Dish, CultureHandler.DishHandler.GetDishes).RequireAuthorization(AuthConstants.Policies.CultureRead);

        return endpoints;
    }
}
