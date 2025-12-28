using DiscoverCostaRica.Geo.Application.Dtos;
using DiscoverCostaRica.Geo.Application.Interfaces;
using DiscoverCostaRica.Shared.Constants;
using DiscoverCostaRica.Shared.Interfaces;
using DiscoverCostaRica.Shared.Responses;

namespace DiscoverCostaRica.Geo.Application.Services;

public class CachedProviceService(IProviceService inner, ICacheService cache) : IProviceService
{
    public Task<Result<ProvinceDto>> GetProvinceById(int provinceId, CancellationToken cancellationToken)
    {
        return inner.GetProvinceById(provinceId, cancellationToken);
    }

    public async Task<Result<List<ProvinceDto>>> GetProvinces(CancellationToken cancellationToken)
    {
        var provincesCache = await cache.Get<Result<List<ProvinceDto>>>(CacheKeys.Geo.PROVINCES, cancellationToken);
        if (provincesCache is not null) return new Success(provincesCache);

        var provinces = await inner.GetProvinces(cancellationToken);

        await cache.Set(CacheKeys.Geo.PROVINCES, provinces, cancellationToken);

        return provinces;
    }
}
