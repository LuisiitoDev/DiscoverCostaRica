using DiscoverCostaRica.Beaches.Application.UseCases.GetBeaches;
using DiscoverCostaRica.BuildingBlocks.Web.Authentication;
using DiscoverCostaRica.BuildingBlocks.Web.Presentation;
using DiscoverCostaRica.BuildingBlocks.Web.Versioning;

namespace DiscoverCostaRica.Beaches.Api.Endpoints;

/// <summary>Beaches v1 routes under <c>/api/v1/beaches</c>, behind the <c>Beaches.Read</c> app role.</summary>
public static class BeachEndpoints
{
    public static IEndpointRouteBuilder MapBeachEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var beaches = endpoints.BuildEndpointGroup(1.0, "beaches")
            .RequireAuthorization(AuthConstants.Policies.BeachesRead);

        beaches.MapGet("/", GetBeaches)
            .WithName("GetBeaches")
            .WithSummary("Retrieve all beaches in Costa Rica")
            .WithDescription("Returns a comprehensive list of beaches located throughout Costa Rica")
            .Produces<V1Envelope<IReadOnlyList<BeachResponse>>>()
            .Produces<V1Envelope<IReadOnlyList<BeachResponse>>>(StatusCodes.Status404NotFound);

        return endpoints;
    }

    private static async Task<IResult> GetBeaches(GetBeachesUseCase useCase, CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(GetBeachesRequest.Instance, cancellationToken)).ToV1Response();
}
