
using DiscoverCostaRica.Api.Models.Dto;
using DiscoverCostaRica.Api.Routes;
using DiscoverCostaRica.Api.Services;

namespace DiscoverCostaRica.Api.Endpoints;

public class Beach : IEndpoint
{
    public static void Register(WebApplication app)
    {
        app.MapGet(EndpointRoutes.BEACH_ROUTES.GET_BEACHES, GetBeaches)
            .WithName("GetBeaches")
            .WithOpenApi(OpenApiBeach.GetBeach)
            .Produces<DtoBeach[]>(StatusCodes.Status200OK);
    }


    private static async Task<IResult> GetBeaches(BeachService service, CancellationToken cancellationToken)
    {
        var result = await service.GetBeaches(cancellationToken);
        if (result.IsSuccess) return Results.Ok(result.Value);
        if (result.IsNotFound) return Results.NotFound(result.Error);
        return Results.BadRequest(result.Error);
    }

}