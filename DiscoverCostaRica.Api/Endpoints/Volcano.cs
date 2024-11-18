
using DiscoverCostaRica.Api.Models.Dto;
using DiscoverCostaRica.Api.Routes;
using DiscoverCostaRica.Api.Services;
using Microsoft.AspNetCore.Mvc;
using DiscoverCostaRica.Api.Configuration;
namespace DiscoverCostaRica.Api.Endpoints;

public class Volcano : IEndpoint
{
    public static void Register(WebApplication app)
    {
        app.MapGet(EndpointRoutes.VOLCANOS_ROUTE.GET_VOLCANOS, GetVolcanos)
           .WithName("GetVolcanos")
           .WithOpenApi(OpenApiConfiguration.OpenApiVocalcano.GetVolcano)
           .Produces<DtoVolcano[]>(StatusCodes.Status200OK);
    }

    public static async Task<IResult> GetVolcanos([FromServices] VolcanoService service, CancellationToken cancellationToken)
    {
        var volcanos = await service.GetVolcanos(cancellationToken);
        if (volcanos.IsNotFound) return Results.NotFound(volcanos.Error);
        if (!volcanos.IsSuccess) return Results.BadRequest("There occurred an error getting volcanoes.");
        return Results.Ok(volcanos.Value);
    }
}
