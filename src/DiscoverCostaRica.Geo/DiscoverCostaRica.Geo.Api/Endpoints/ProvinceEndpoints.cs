using DiscoverCostaRica.BuildingBlocks.Web.Presentation;
using DiscoverCostaRica.Geo.Application.UseCases;
using DiscoverCostaRica.Geo.Application.UseCases.GetProvinceById;
using DiscoverCostaRica.Geo.Application.UseCases.GetProvinces;

namespace DiscoverCostaRica.Geo.Api.Endpoints;

/// <summary>Controllers for provinces: parse the route, run one use case, hand the result to the presenter.</summary>
internal static class ProvinceEndpoints
{
    public static RouteGroupBuilder MapProvinceEndpoints(this RouteGroupBuilder geo)
    {
        geo.MapGet(GeoRoutes.Provinces, GetProvinces)
            .WithName("GetProvinces")
            .WithSummary("All provinces of Costa Rica")
            .Produces<V1Envelope<IReadOnlyList<DivisionResponse>>>()
            .Produces<V1Envelope<IReadOnlyList<DivisionResponse>>>(StatusCodes.Status404NotFound);

        geo.MapGet(GeoRoutes.ProvinceById, GetProvinceById)
            .WithName("GetProvinceById")
            .WithSummary("One province by its code (1-7)")
            .Produces<V1Envelope<DivisionResponse>>()
            .Produces<V1Envelope<DivisionResponse>>(StatusCodes.Status404NotFound);

        return geo;
    }

    private static async Task<IResult> GetProvinces(GetProvincesUseCase useCase, CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(GetProvincesRequest.Instance, cancellationToken)).ToV1Response();

    private static async Task<IResult> GetProvinceById(int provinceId, GetProvinceByIdUseCase useCase, CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new GetProvinceByIdRequest(provinceId), cancellationToken)).ToV1Response();
}
