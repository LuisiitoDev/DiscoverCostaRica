using System.Globalization;
using DiscoverCostaRica.BuildingBlocks.Web.Presentation;
using DiscoverCostaRica.Geo.Application.UseCases;
using DiscoverCostaRica.Geo.Application.UseCases.GetDistrictById;
using DiscoverCostaRica.Geo.Application.UseCases.GetDistrictsByCanton;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverCostaRica.Geo.Api.Endpoints;

internal static class DistrictEndpoints
{
    public static RouteGroupBuilder MapDistrictEndpoints(this RouteGroupBuilder geo)
    {
        geo.MapGet(GeoRoutes.DistrictsOfCanton, GetDistrictsDeprecated)
            .WithName("GetDistrictsOfCanton")
            .WithSummary("Deprecated: districts by canton number. Pass ?provinceId= or use /provinces/{p}/cantons/{c}/districts")
            .Produces<V1Envelope<IReadOnlyList<DivisionResponse>>>()
            .Produces<V1Envelope<IReadOnlyList<DivisionResponse>>>(StatusCodes.Status404NotFound);

        geo.MapGet(GeoRoutes.DistrictById, GetDistrictDeprecated)
            .WithName("GetDistrictById")
            .WithSummary("Deprecated: district by canton and district number. Pass ?provinceId= or use /provinces/{p}/cantons/{c}/districts/{d}")
            .Produces<V1Envelope<DivisionResponse>>()
            .Produces<V1Envelope<DivisionResponse>>(StatusCodes.Status404NotFound);

        geo.MapGet(GeoRoutes.CantonDistricts, GetDistricts)
            .WithName("GetCantonDistricts")
            .WithSummary("Districts of a canton")
            .Produces<V1Envelope<IReadOnlyList<DivisionResponse>>>()
            .Produces<V1Envelope<IReadOnlyList<DivisionResponse>>>(StatusCodes.Status404NotFound);

        geo.MapGet(GeoRoutes.CantonDistrict, GetDistrict)
            .WithName("GetCantonDistrict")
            .WithSummary("One district of a canton")
            .Produces<V1Envelope<DivisionResponse>>()
            .Produces<V1Envelope<DivisionResponse>>(StatusCodes.Status404NotFound);

        return geo;
    }

    private static async Task<IResult> GetDistricts(
        int provinceId, int cantonId, GetDistrictsByCantonUseCase useCase, CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new GetDistrictsByCantonRequest(provinceId, cantonId), cancellationToken)).ToV1Response();

    private static async Task<IResult> GetDistrict(
        int provinceId, int cantonId, int districtId, GetDistrictByIdUseCase useCase, CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new GetDistrictByIdRequest(provinceId, cantonId, districtId), cancellationToken)).ToV1Response();

    private static async Task<IResult> GetDistrictsDeprecated(
        int cantonId, [FromQuery] int? provinceId, GetDistrictsByCantonUseCase useCase, HttpResponse response, CancellationToken cancellationToken)
    {
        MarkDeprecated(response, $"/api/v1/geo/provinces/{Province(provinceId)}/cantons/{cantonId}/districts");
        return (await useCase.ExecuteAsync(new GetDistrictsByCantonRequest(provinceId, cantonId), cancellationToken)).ToV1Response();
    }

    private static async Task<IResult> GetDistrictDeprecated(
        int cantonId, int districtId, [FromQuery] int? provinceId, GetDistrictByIdUseCase useCase, HttpResponse response, CancellationToken cancellationToken)
    {
        MarkDeprecated(response, $"/api/v1/geo/provinces/{Province(provinceId)}/cantons/{cantonId}/districts/{districtId}");
        return (await useCase.ExecuteAsync(new GetDistrictByIdRequest(provinceId, cantonId, districtId), cancellationToken)).ToV1Response();
    }

    private static string Province(int? provinceId) =>
        provinceId?.ToString(CultureInfo.InvariantCulture) ?? "{provinceId}";

    /// <summary>Deprecation signalling per RFC 9745 (Deprecation) and RFC 8288 (Link).</summary>
    private static void MarkDeprecated(HttpResponse response, string successor)
    {
        response.Headers["Deprecation"] = "true";
        response.Headers.Link = $"<{successor}>; rel=\"successor-version\"";
    }
}
