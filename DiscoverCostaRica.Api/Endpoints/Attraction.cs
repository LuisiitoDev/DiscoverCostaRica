
using DiscoverCostaRica.Api.Routes;
using DiscoverCostaRica.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverCostaRica.Api.Endpoints;
public class Attraction : IEndpoint
{
    public static void Register(WebApplication app)
    {
        app.MapGet(EndpointRoutes.ATTRACTIONS_ROUTE.GET_ATTRACTIONS, GetAttractions);
    }

    private static async Task<IResult> GetAttractions(
        [FromServices] AttractionService service,
        [FromQuery] int provinceId, CancellationToken cancellationToken)
    {
        var result = await service.GetAttractions(provinceId, cancellationToken);
        if (!result.IsSuccess) return Results.BadRequest();
        if (result.IsNotFound) return Results.NotFound();
        return Results.Ok(result.Value);
    }
}