using DiscoverCostaRica.Culture.Api.Handlers;

namespace DiscoverCostaRica.Culture.Api.Extensions;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapCultureEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var groups = endpoints.MapGroup("api/v1/traditions");

        var traditions = groups.MapGroup("/tradition");
        traditions.MapGet("/", CultureHandler.TraditionHandler.GetTraditions);

        var dishes = groups.MapGroup("/dish");
        dishes.MapGet("/", CultureHandler.DishHandler.GetDishes);


        return endpoints;
    }
}
