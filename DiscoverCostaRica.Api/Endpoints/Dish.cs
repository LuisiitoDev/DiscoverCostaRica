
using DiscoverCostaRica.Api.Routes;
using DiscoverCostaRica.Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverCostaRica.Api.Endpoints;

public class Dish : IEndpoint
{
    public static void Register(WebApplication app)
    {
        app.MapGet(EndpointRoutes.DISH_ROUTES.GET_DISHES, GetDishes);
    }

    public static async Task<IResult> GetDishes(DishService service, CancellationToken cancellationToken)
    {
        var dishes = await service.GetDishesAsync(cancellationToken);
        if(!dishes.IsSuccess) return Results.BadRequest("It ran an error.");
        if(dishes.IsNotFound) return Results.NotFound("Dishes were not found.");
        return Results.Ok(dishes.Value);
    }
}