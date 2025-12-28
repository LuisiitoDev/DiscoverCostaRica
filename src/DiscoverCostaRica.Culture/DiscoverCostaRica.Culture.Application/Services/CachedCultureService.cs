using DiscoverCostaRica.Culture.Application.Dtos;
using DiscoverCostaRica.Culture.Application.Interfaces;
using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Shared.Constants;
using DiscoverCostaRica.Shared.Interfaces;
using DiscoverCostaRica.Shared.Responses;
using Microsoft.AspNetCore.Http;

namespace DiscoverCostaRica.Culture.Application.Services;

[DecoratorService]
public class CachedCultureService(ICultureService inner, ICacheService cache) : ICultureService
{
    public async Task<Result<List<DtoDish>>> GetDishes(CancellationToken cancellationToken)
    {
        var cachedDishes = await cache.Get<Result<List<DtoDish>>>(CacheKeys.Culture.DISHES, cancellationToken);
        if (cachedDishes is not null) return cachedDishes;

        var dishes = await inner.GetDishes(cancellationToken);

        if (dishes.StatusCode == StatusCodes.Status200OK)
            await cache.Set(CacheKeys.Culture.DISHES, dishes, cancellationToken);

        return dishes;
    }

    public async Task<Result<List<DtoTradition>>> GetTraditions(CancellationToken cancellationToken)
    {
        var cachedTraditions = await cache.Get<Result<List<DtoTradition>>>(CacheKeys.Culture.TRADITIONS, cancellationToken);
        if (cachedTraditions is not null) return cachedTraditions;

        var traditions = await inner.GetTraditions(cancellationToken);

        if (traditions.StatusCode == StatusCodes.Status200OK)
            await cache.Set(CacheKeys.Culture.TRADITIONS, traditions, cancellationToken);

        return traditions;
    }
}
