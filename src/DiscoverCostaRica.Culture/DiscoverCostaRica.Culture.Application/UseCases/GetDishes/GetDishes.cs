using DiscoverCostaRica.BuildingBlocks.Application.Caching;
using DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection;
using DiscoverCostaRica.BuildingBlocks.Application.Results;
using DiscoverCostaRica.BuildingBlocks.Application.UseCases;
using DiscoverCostaRica.Culture.Application.Ports;
using Microsoft.Extensions.Caching.Hybrid;

namespace DiscoverCostaRica.Culture.Application.UseCases.GetDishes;

public sealed record GetDishesRequest
{
    public static GetDishesRequest Instance { get; } = new();
}

public sealed record DishResponse(int Id, string Name, string Description, string Ingredients, string Preparation, string? ImageUrl);

[TransientService]
public sealed class GetDishesInteractor(ICultureReadRepository repository)
    : IUseCase<GetDishesRequest, IReadOnlyList<DishResponse>>
{
    public const string NotFoundMessage = "No dishes found. Please check later for updates.";

    public async Task<Result<IReadOnlyList<DishResponse>>> ExecuteAsync(GetDishesRequest request, CancellationToken cancellationToken)
    {
        var dishes = await repository.GetDishesAsync(cancellationToken);

        return dishes.Count == 0
            ? Error.NotFound(NotFoundMessage)
            : Result<IReadOnlyList<DishResponse>>.Success(
                [.. dishes.Select(d => new DishResponse(d.Id, d.Name, d.Description, d.Ingredients, d.Preparation, d.ImageUrl))]);
    }
}

/// <summary>Cached for the default TTL (CUL-D2).</summary>
[DecoratorService]
public sealed class CachedGetDishes(IUseCase<GetDishesRequest, IReadOnlyList<DishResponse>> inner, HybridCache cache)
    : CachedUseCase<GetDishesRequest, IReadOnlyList<DishResponse>>(inner, cache)
{
    public const string Key = "culture:dishes";

    protected override string KeyFor(GetDishesRequest request) => Key;

    protected override IEnumerable<string> Tags => ["culture"];
}
