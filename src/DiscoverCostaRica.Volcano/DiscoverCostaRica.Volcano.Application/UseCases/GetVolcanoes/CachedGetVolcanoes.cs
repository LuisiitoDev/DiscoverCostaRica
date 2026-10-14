using DiscoverCostaRica.BuildingBlocks.Application.Caching;
using DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection;
using DiscoverCostaRica.BuildingBlocks.Application.UseCases;
using Microsoft.Extensions.Caching.Hybrid;

namespace DiscoverCostaRica.Volcano.Application.UseCases.GetVolcanoes;

/// <summary>The enriched list is cached for an hour, so Geo name changes show up the same day (VOL-D8).</summary>
[DecoratorService]
public sealed class CachedGetVolcanoes(IUseCase<GetVolcanoesRequest, IReadOnlyList<VolcanoResponse>> inner, HybridCache cache)
    : CachedUseCase<GetVolcanoesRequest, IReadOnlyList<VolcanoResponse>>(inner, cache)
{
    public const string Key = "volcano:volcanoes";

    protected override string KeyFor(GetVolcanoesRequest request) => Key;

    protected override HybridCacheEntryOptions EntryOptions { get; } = new()
    {
        Expiration = TimeSpan.FromHours(1),
        LocalCacheExpiration = TimeSpan.FromMinutes(5),
    };

    protected override IEnumerable<string> Tags => ["volcano"];
}
