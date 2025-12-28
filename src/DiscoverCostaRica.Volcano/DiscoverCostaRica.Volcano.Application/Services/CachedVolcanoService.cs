using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Shared.Constants;
using DiscoverCostaRica.Shared.Interfaces;
using DiscoverCostaRica.Shared.Responses;
using DiscoverCostaRica.Volcano.Application.Dtos;
using DiscoverCostaRica.VolcanoService.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DiscoverCostaRica.Volcano.Application.Services;

[DecoratorService]
public class CachedVolcanoService(IVolcanoService inner, ICacheService cache) : IVolcanoService
{
    public async Task<Result<DtoVolcano>> GetVolcanoById(int id, CancellationToken cancellationToken)
    {
        return await inner.GetVolcanoById(id, cancellationToken);
    }

    public async Task<Result<IEnumerable<DtoVolcano>>> GetVolcanos(CancellationToken cancellationToken)
    {
        var cachedVolcanos = await cache.Get<Result<IEnumerable<DtoVolcano>>>(CacheKeys.Volcano.VOLCANOS, cancellationToken);
        if (cachedVolcanos is not null) return cachedVolcanos;

        var volcanos = await inner.GetVolcanos(cancellationToken);

        if (volcanos.StatusCode == StatusCodes.Status200OK)
            await cache.Set(CacheKeys.Volcano.VOLCANOS, volcanos, cancellationToken);

        return volcanos;
    }

    public async Task<Result<List<DtoVolcano>>> GetVolcanosByProvince(int provinceId, CancellationToken cancellationToken)
    {
        return await inner.GetVolcanosByProvince(provinceId, cancellationToken);
    }
}
