using DiscoverCostaRica.Domain.Constants;
using DiscoverCostaRica.Infraestructure.Data.Context;
using Microsoft.EntityFrameworkCore;
namespace DiscoverCostaRica.Functions.Services;

public class ProvinceService(CacheService cache, DiscoverCostaRicaContext context)
{
    public async Task Sync()
    {
        var provinces = await context.Provinces.ToArrayAsync();
        await cache.AddAsync(CacheKeys.ALL_PROVINCES, provinces);
    }
}
