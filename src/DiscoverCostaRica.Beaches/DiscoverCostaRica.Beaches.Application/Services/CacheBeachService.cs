using DiscoverCostaRica.Beaches.Application.Dtos;
using DiscoverCostaRica.Beaches.Application.Interfaces;
using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Shared.Constants;
using DiscoverCostaRica.Shared.Interfaces;
using DiscoverCostaRica.Shared.Responses;
using Microsoft.AspNetCore.Http;
namespace DiscoverCostaRica.Beaches.Application.Services;

[DecoratorService]
public class CacheBeachService(IBeachService inner, ICacheService cache) : IBeachService
{
    public async Task<Result<List<DtoBeach>>> GetBeaches(CancellationToken cancellationToken)
    {
        var cachedBeaches = await cache.Get<Result<List<DtoBeach>>>(CacheKeys.Beach.BEACHES, cancellationToken);
        if (cachedBeaches is not null) return cachedBeaches;

        var beaches = await inner.GetBeaches(cancellationToken);

        if (beaches.StatusCode == StatusCodes.Status200OK)
            await cache.Set(CacheKeys.Beach.BEACHES, beaches, cancellationToken);

        return beaches;
    }
}
