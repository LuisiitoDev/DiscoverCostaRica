using DiscoverCostaRica.BuildingBlocks.Application.Caching;
using DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection;
using DiscoverCostaRica.BuildingBlocks.Application.UseCases;
using Microsoft.Extensions.Caching.Hybrid;

namespace DiscoverCostaRica.Geo.Application.UseCases.GetProvinces;

/// <summary>Provinces almost never change: serve them from the cache for the default TTL (GEO-D5).</summary>
[DecoratorService]
public sealed class CachedGetProvinces(IUseCase<GetProvincesRequest, IReadOnlyList<DivisionResponse>> inner, HybridCache cache)
    : CachedUseCase<GetProvincesRequest, IReadOnlyList<DivisionResponse>>(inner, cache)
{
    public const string Key = "geo:provinces";

    protected override string KeyFor(GetProvincesRequest request) => Key;

    protected override IEnumerable<string> Tags => ["geo"];
}
