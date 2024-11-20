using DiscoverCostaRica.Domain.Constants;
using DiscoverCostaRica.Infraestructure.Data.Context;
using Microsoft.EntityFrameworkCore;


namespace DiscoverCostaRica.Functions.Services;

public class DishService(CacheService cache, DiscoverCostaRicaContext context)
{
    public async Task Sync()
    {
        var dishes = await context.Dishes.ToArrayAsync();
        await cache.AddAsync(CacheKeys.ALL_DISHES, dishes);
    }
}
