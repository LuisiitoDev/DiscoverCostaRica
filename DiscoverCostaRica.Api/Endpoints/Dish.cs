
using DiscoverCostaRica.Api.Models.Dto;
using DiscoverCostaRica.Api.Routes;
using DiscoverCostaRica.Api.Services;

namespace DiscoverCostaRica.Api.Endpoints;

public class Dish : IEndpoint
{
    public static void Register(WebApplication app)
    {
        app.MapGet(EndpointRoutes.DISH_ROUTES.GET_DISHES, GetDishes)
            .WithName("GetDishes")
            .WithOpenApi(OpenApiDish.GetDish)
            .Produces<DtoDish[]>(StatusCodes.Status200OK);
    }

    public static async Task<IResult> GetDishes(DishService service, CancellationToken cancellationToken)
    {
        var dishes = await service.GetDishesAsync(cancellationToken);
        if (!dishes.IsSuccess) return Results.BadRequest("It ran an error.");
        if (dishes.IsNotFound) return Results.NotFound("Dishes were not found.");
        return Results.Ok(dishes.Value);
    }
}