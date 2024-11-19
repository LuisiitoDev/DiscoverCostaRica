using DiscoverCostaRica.Domain.Constants;
using DiscoverCostaRica.Infraestructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Functions.Services;

public class BeachService(CacheService cache, DiscoverCostaRicaContext context)
{
    public async Task Sync()
    {
        var beaches = await context.Beaches.ToArrayAsync();
        await cache.AddAsync(CacheKeys.ALL_BEACHES, beaches);
    }
}
