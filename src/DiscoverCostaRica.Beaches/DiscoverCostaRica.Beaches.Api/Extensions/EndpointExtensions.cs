using DiscoverCostaRica.Beaches.Api.Handlers;
using DiscoverCostaRica.Beaches.Application.Dtos;
using DiscoverCostaRica.Shared.Responses;

namespace DiscoverCostaRica.Beaches.Api.Extensions;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapBeachEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var groups = endpoints.MapGroup("api/beaches").HasApiVersion(1.0);

        groups.MapGet("/", BeachHandler.ExecuteAsync)
            .WithName("GetBeaches")
            .WithSummary("Retrieve all beaches in Costa Rica")
            .WithDescription(
                "Returns a comprehensive list of beaches located throughout Costa Rica, " +
                "including details such as location, characteristics, and available amenities.")
            .Produces<Result<List<DtoBeach>>>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        return endpoints;
    }
}
