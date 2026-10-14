using DiscoverCostaRica.BuildingBlocks.Web.Authentication;
using DiscoverCostaRica.BuildingBlocks.Web.Presentation;
using DiscoverCostaRica.BuildingBlocks.Web.Versioning;
using DiscoverCostaRica.Volcano.Application.UseCases;
using DiscoverCostaRica.Volcano.Application.UseCases.GetVolcanoById;
using DiscoverCostaRica.Volcano.Application.UseCases.GetVolcanoes;
using DiscoverCostaRica.Volcano.Application.UseCases.GetVolcanoesByProvince;

namespace DiscoverCostaRica.Volcano.Api.Endpoints;

/// <summary>Volcano v1 routes under <c>/api/v1/volcanoes</c>, all GET-only and behind the <c>Volcano.Read</c> app role.</summary>
public static class VolcanoEndpoints
{
    private const string Group = "volcanoes";

    public static IEndpointRouteBuilder MapVolcanoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var volcanoes = endpoints.BuildEndpointGroup(1.0, Group)
            .RequireAuthorization(AuthConstants.Policies.VolcanoRead);

        // MapGet, not Map: other verbs get 405 (VOL-D2). Ids come from the path (VOL-D1).
        volcanoes.MapGet("/", GetVolcanoes)
            .WithName("GetVolcanoes")
            .WithSummary("All volcanoes with their location")
            .Produces<V1Envelope<IReadOnlyList<VolcanoResponse>>>()
            .Produces<V1Envelope<IReadOnlyList<VolcanoResponse>>>(StatusCodes.Status404NotFound);

        volcanoes.MapGet("/{id:int}", GetVolcanoById)
            .WithName("GetVolcanoById")
            .WithSummary("One volcano with its location")
            .Produces<V1Envelope<VolcanoResponse>>()
            .Produces<V1Envelope<VolcanoResponse>>(StatusCodes.Status404NotFound);

        volcanoes.MapGet("/province/{provinceId:int}", GetVolcanoesByProvince)
            .WithName("GetVolcanoesByProvince")
            .WithSummary("Volcanoes of a province with their location")
            .Produces<V1Envelope<IReadOnlyList<VolcanoResponse>>>()
            .Produces<V1Envelope<IReadOnlyList<VolcanoResponse>>>(StatusCodes.Status404NotFound);

        return endpoints;
    }

    private static async Task<IResult> GetVolcanoes(GetVolcanoesUseCase useCase, CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(GetVolcanoesRequest.Instance, cancellationToken)).ToV1Response();

    private static async Task<IResult> GetVolcanoById(int id, GetVolcanoByIdUseCase useCase, CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new GetVolcanoByIdRequest(id), cancellationToken)).ToV1Response();

    private static async Task<IResult> GetVolcanoesByProvince(int provinceId, GetVolcanoesByProvinceUseCase useCase, CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new GetVolcanoesByProvinceRequest(provinceId), cancellationToken)).ToV1Response();
}
