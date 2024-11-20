using DiscoverCostaRica.Domain.Constants;
using DiscoverCostaRica.Infraestructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Functions.Services;

public class VolcanoService(CacheService cache, DiscoverCostaRicaContext context)
{
    public async Task Sync()
    {
        var volcanos = await context.Volcanos.ToArrayAsync();
        await cache.AddAsync(CacheKeys.ALL_VOLCANOS, volcanos);
    }
}
