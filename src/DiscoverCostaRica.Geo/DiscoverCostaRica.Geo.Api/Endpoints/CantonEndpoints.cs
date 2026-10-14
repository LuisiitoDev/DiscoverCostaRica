using DiscoverCostaRica.BuildingBlocks.Web.Presentation;
using DiscoverCostaRica.Geo.Application.UseCases;
using DiscoverCostaRica.Geo.Application.UseCases.GetCantonById;
using DiscoverCostaRica.Geo.Application.UseCases.GetCantonsByProvince;

namespace DiscoverCostaRica.Geo.Api.Endpoints;

internal static class CantonEndpoints
{
    public static RouteGroupBuilder MapCantonEndpoints(this RouteGroupBuilder geo)
    {
        foreach (var (route, name) in new[] { (GeoRoutes.CantonsOfProvince, "GetCantonsOfProvince"), (GeoRoutes.ProvinceCantons, "GetProvinceCantons") })
        {
            geo.MapGet(route, GetCantons)
                .WithName(name)
                .WithSummary("Cantons of a province")
                .Produces<V1Envelope<IReadOnlyList<DivisionResponse>>>()
                .Produces<V1Envelope<IReadOnlyList<DivisionResponse>>>(StatusCodes.Status404NotFound);
        }

        foreach (var (route, name) in new[] { (GeoRoutes.CantonById, "GetCantonById"), (GeoRoutes.ProvinceCanton, "GetProvinceCanton") })
        {
            geo.MapGet(route, GetCanton)
                .WithName(name)
                .WithSummary("One canton of a province")
                .Produces<V1Envelope<DivisionResponse>>()
                .Produces<V1Envelope<DivisionResponse>>(StatusCodes.Status404NotFound);
        }

        return geo;
    }

    private static async Task<IResult> GetCantons(int provinceId, GetCantonsByProvinceUseCase useCase, CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new GetCantonsByProvinceRequest(provinceId), cancellationToken)).ToV1Response();

    private static async Task<IResult> GetCanton(int provinceId, int cantonId, GetCantonByIdUseCase useCase, CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new GetCantonByIdRequest(provinceId, cantonId), cancellationToken)).ToV1Response();
}
