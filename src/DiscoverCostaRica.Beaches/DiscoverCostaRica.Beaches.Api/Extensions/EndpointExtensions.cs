using DiscoverCostaRica.Beaches.Api.Handlers;
using DiscoverCostaRica.Beaches.Application.Dtos;
using DiscoverCostaRica.Shared.ApiVersioning;
using DiscoverCostaRica.Shared.Authentication;
using DiscoverCostaRica.Shared.Responses;
using DiscoverCostaRica.Shared.Routes;

namespace DiscoverCostaRica.Beaches.Api.Extensions;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapBeachEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var beaches = endpoints.BuildEndpointGroup(1.0, RoutesConstants.Beaches.Group);

        beaches.MapGet(RoutesConstants.Beaches.Beach, BeachHandler.ExecuteAsync)
            .WithName("GetBeaches")
            .WithSummary("Retrieve all beaches in Costa Rica")
            .WithDescription("Returns a comprehensive list of beaches located throughout Costa Rica")
            .Produces<Result<List<DtoBeach>>>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .RequireAuthorization(AuthConstants.Policies.BeachesRead);

        return endpoints;
    }
}
