using DiscoverCostaRica.Geo.Application.Dtos;
using DiscoverCostaRica.Geo.Application.Interfaces;
using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Shared.Constants;
using DiscoverCostaRica.Shared.Interfaces;
using DiscoverCostaRica.Shared.Responses;
using Microsoft.AspNetCore.Http;

namespace DiscoverCostaRica.Geo.Application.Services;

[DecoratorService]
public class CachedProviceService(IProvinceService inner, ICacheService cache) : IProvinceService
{
    public Task<Result<ProvinceDto>> GetProvinceById(int provinceId, CancellationToken cancellationToken)
    {
        return inner.GetProvinceById(provinceId, cancellationToken);
    }

    public async Task<Result<List<ProvinceDto>>> GetProvinces(CancellationToken cancellationToken)
    {
        var provincesCache = await cache.Get<Result<List<ProvinceDto>>>(CacheKeys.Geo.PROVINCES, cancellationToken);
        if (provincesCache is not null) return provincesCache;

        var provinces = await inner.GetProvinces(cancellationToken);

        if(provinces.StatusCode == StatusCodes.Status200OK)
            await cache.Set(CacheKeys.Geo.PROVINCES, provinces, cancellationToken);

        return provinces;
    }
}
