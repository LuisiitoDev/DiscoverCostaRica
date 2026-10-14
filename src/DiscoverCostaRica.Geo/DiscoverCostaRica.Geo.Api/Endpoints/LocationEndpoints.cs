using DiscoverCostaRica.BuildingBlocks.Web.Presentation;
using DiscoverCostaRica.Contracts.Geo;
using DiscoverCostaRica.Geo.Application.UseCases;
using DiscoverCostaRica.Geo.Application.UseCases.ResolveLocations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DiscoverCostaRica.Geo.Api.Endpoints;

/// <summary>Adapter for the internal batch contract: Contracts.Geo wire shapes in and out, use-case models inside.</summary>
internal static class LocationEndpoints
{
    public static RouteGroupBuilder MapLocationEndpoints(this RouteGroupBuilder geo)
    {
        geo.MapPost(GeoRoutes.ResolveLocations, Resolve)
            .WithName("ResolveLocations")
            .WithSummary($"Names for up to {GeoContract.MaxResolveBatchSize} locations in one call (internal)")
            .Accepts<Contracts.Geo.ResolveLocationsRequest>("application/json")
            .Produces<V1Envelope<Contracts.Geo.ResolveLocationsResponse>>()
            .Produces<V1Envelope<Contracts.Geo.ResolveLocationsResponse>>(StatusCodes.Status400BadRequest);

        return geo;
    }

    private static async Task<IResult> Resolve(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] Contracts.Geo.ResolveLocationsRequest? body,
        ResolveLocationsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var request = new Application.UseCases.ResolveLocations.ResolveLocationsRequest(
            body?.Items?.Select(i => new LocationQuery(i.ProvinceId, i.CantonId, i.DistrictId)).ToList());

        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return result.Map(ToContract).ToV1Response();
    }

    private static Contracts.Geo.ResolveLocationsResponse ToContract(Application.UseCases.ResolveLocations.ResolveLocationsResponse response) =>
        new([.. response.Items.Select(i => new ResolvedLocationDto(ToContract(i.Province), ToContract(i.Canton), ToContract(i.District)))]);

    private static NamedRefDto? ToContract(DivisionResponse? division) =>
        division is null ? null : new NamedRefDto(division.Id, division.Name);
}
