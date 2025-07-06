using AutoMapper;
using DiscoverCostaRica.Api.Models;
using DiscoverCostaRica.Api.Models.Dto;
using DiscoverCostaRica.Infraestructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Api.Services;

public class BeachService(DiscoverCostaRicaContext context, RedisCacheService cache, IMapper map)
{
    public async Task<Result<DtoBeach[]>> GetBeaches(CancellationToken cancellationToken)
    {
        // TODO: No cache needed.

        var beaches = await context.Beaches.ToArrayAsync(cancellationToken);
        return beaches.Length > 0 ?
            map.Map<DtoBeach[]>(beaches) :
            Result<DtoBeach[]>.NotFound("No beaches found. Coming soon!");
    }
}