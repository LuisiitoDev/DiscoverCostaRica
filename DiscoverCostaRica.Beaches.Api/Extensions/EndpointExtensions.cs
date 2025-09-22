using DiscoverCostaRica.Beaches.Api.Handlers;
using DiscoverCostaRica.Beaches.Application.Dtos;
using DiscoverCostaRica.Shared.Responses;

namespace DiscoverCostaRica.Beaches.Api.Extensions;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapBeachEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var groups = endpoints.MapGroup("api/v1/beaches");

        groups.MapGet("/", BeachHandler.ExecuteAsync)
            .WithName("Get beaches")
            .WithSummary("Get Costa Rica beaches")
            .WithDescription("Get all the beaches around Costa Rica")
            .Produces<Result<List<DtoBeach>>>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        return endpoints;
    }
}
