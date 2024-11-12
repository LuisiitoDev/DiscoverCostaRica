using DiscoverCostaRica.Api.Models;
using DiscoverCostaRica.Domain.Entities.Direction;
using DiscoverCostaRica.Infraestructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Api.Services;

public class DirectionService(DiscoverCostaRicaContext context)
{
    public async Task<Result<Province[]>> GetProvinces(CancellationToken cancellationToken)
    {
        var provinces = await context.Provinces.ToArrayAsync(cancellationToken);
        return provinces.Length > 0 ? provinces : Result<Province[]>.NotFound("No provinces found.");
    }

    public async Task<Result<Canton[]>> GetCantons(int provinceId, CancellationToken cancellationToken)
    {
        var cantons = await context.Provinces
        .Where(p => p.Id == provinceId)
        .SelectMany(p => p.Cantons)
        .ToArrayAsync(cancellationToken);

        return cantons.Length > 0 ? cantons : Result<Canton[]>.NotFound("No cantons found.");
    }

    public async Task<Result<District[]>> GetDistricts(int provinceId, int cantonId, CancellationToken cancellationToken)
    {
        var districts = await context.Provinces
        .Include(p => p.Cantons))

        return districts.Length > 0 ? districts : Result<District[]>.NotFound("No districts found.");
    }

    public async Task<Result<ProvinceDetail>> GetProvinceDetail(short provinceId, CancellationToken cancellationToken)
    {
        return await context.ProvinceDetails.FirstAsync(p => p.ProvinceId == provinceId, cancellationToken);
    }
}