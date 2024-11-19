using AutoMapper;
using DiscoverCostaRica.Api.Models;
using DiscoverCostaRica.Api.Models.Dto;
using DiscoverCostaRica.Infraestructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Api.Services;

public class VolcanoService(DiscoverCostaRicaContext context, RedisCacheService cache, IMapper mapper)
{
    public async Task<Result<DtoVolcano[]>> GetVolcanos(CancellationToken cancellationToken)
    {
        if (await cache.ContainsKeyAsync(CacheKeys.ALL_VOLCANOS))
            return await cache.GetAsync<DtoVolcano[]>(CacheKeys.ALL_VOLCANOS);

        var volcanos = await context.Volcanos.ToArrayAsync(cancellationToken);
        return volcanos.Length > 0 ?
            mapper.Map<DtoVolcano[]>(volcanos) :
            Result<DtoVolcano[]>.NotFound("Volcanos not found");
    }
}
