using DiscoverCostaRica.BuildingBlocks.Web.Authentication;
using DiscoverCostaRica.BuildingBlocks.Web.Presentation;
using DiscoverCostaRica.BuildingBlocks.Web.Versioning;
using DiscoverCostaRica.Culture.Application.UseCases.GetDishes;
using DiscoverCostaRica.Culture.Application.UseCases.GetTraditions;

namespace DiscoverCostaRica.Culture.Api.Endpoints;

/// <summary>
/// Culture v1 routes, behind the <c>Culture.Read</c> app role. As built, both live under the
/// <c>traditions</c> group (CUL-D1; a v2 would split them).
/// </summary>
public static class CultureEndpoints
{
    public static IEndpointRouteBuilder MapCultureEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var culture = endpoints.BuildEndpointGroup(1.0, "traditions")
            .RequireAuthorization(AuthConstants.Policies.CultureRead);

        culture.MapGet("/tradition", GetTraditions)
            .WithName("GetTraditions")
            .WithSummary("Costa Rican traditions")
            .Produces<V1Envelope<IReadOnlyList<TraditionResponse>>>()
            .Produces<V1Envelope<IReadOnlyList<TraditionResponse>>>(StatusCodes.Status404NotFound);

        culture.MapGet("/dish", GetDishes)
            .WithName("GetDishes")
            .WithSummary("Typical Costa Rican dishes")
            .Produces<V1Envelope<IReadOnlyList<DishResponse>>>()
            .Produces<V1Envelope<IReadOnlyList<DishResponse>>>(StatusCodes.Status404NotFound);

        return endpoints;
    }

    private static async Task<IResult> GetTraditions(GetTraditionsUseCase useCase, CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(GetTraditionsRequest.Instance, cancellationToken)).ToV1Response();

    private static async Task<IResult> GetDishes(GetDishesUseCase useCase, CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(GetDishesRequest.Instance, cancellationToken)).ToV1Response();
}
