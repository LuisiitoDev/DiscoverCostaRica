using DiscoverCostaRica.Culture.Api.Handlers;
using DiscoverCostaRica.Shared.ApiVersioning;

namespace DiscoverCostaRica.Culture.Api.Extensions;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapCultureEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var versionSet = endpoints.CreateGlobalVersionSet();
        var groups = endpoints.MapGroup("api/v{version:apiVersion}/traditions")
                              .WithApiVersionSet(versionSet)
                              .MapToApiVersion(1.0);

        var traditions = groups.MapGroup("/tradition");
        traditions.MapGet("/", CultureHandler.TraditionHandler.GetTraditions);

        var dishes = groups.MapGroup("/dish");
        dishes.MapGet("/", CultureHandler.DishHandler.GetDishes);


        return endpoints;
    }
}
