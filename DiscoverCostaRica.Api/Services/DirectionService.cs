using AutoMapper;
using DiscoverCostaRica.Api.Models;
using DiscoverCostaRica.Api.Models.Dto;
using DiscoverCostaRica.Infraestructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Api.Services;

public class DirectionService(DiscoverCostaRicaContext context, RedisCacheService cacheService, IMapper mapper)
{
    public async Task<Result<DtoProvince[]>> GetProvinces(CancellationToken cancellationToken)
    {
        if (await cacheService.ContainsKeyAsync(CacheKeys.ALL_PROVINCES))
            return await cacheService.GetAsync<DtoProvince[]>(CacheKeys.ALL_PROVINCES);

        var provinces = await context.Provinces.ToArrayAsync(cancellationToken);
        return provinces.Length > 0 ?
            mapper.Map<DtoProvince[]>(provinces) :
            Result<DtoProvince[]>.NotFound("No provinces found.");
    }

    public async Task<Result<DtoCanton[]>> GetCantons(int provinceId, CancellationToken cancellationToken)
    {
        if (await cacheService.ContainsKeyAsync(CacheKeys.Cantons))
            return await cacheService.GetAsync<DtoCanton[]>(CacheKeys.Cantons);

        var cantons = await context.Provinces
        .Include(p => p.Cantons)
        .Where(p => p.Id == provinceId)
        .SelectMany(p => p.Cantons)
        .ToArrayAsync(cancellationToken);

        return cantons.Length > 0 ?
            mapper.Map<DtoCanton[]>(cantons) :
            Result<DtoCanton[]>.NotFound("No cantons found");
    }

    public async Task<Result<DtoDistrict[]>> GetDistricts(int provinceId, int cantonId, CancellationToken cancellationToken)
    {
        var districts = await context.Cantons
        .Include(p => p.Districts)
        .Where(p => p.Id == cantonId && p.ProvinceId == provinceId)
        .SelectMany(p => p.Districts)
        .ToArrayAsync(cancellationToken);

        return districts.Length > 0 ?
            mapper.Map<DtoDistrict[]>(districts) :
            Result<DtoDistrict[]>.NotFound("No districts found");
    }
}