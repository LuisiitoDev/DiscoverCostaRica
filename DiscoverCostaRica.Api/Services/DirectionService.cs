using DiscoverCostaRica.Api.Models;
using DiscoverCostaRica.Domain.Entities;
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
        .Include(p => p.Cantons)
        .Where(p => p.Id == provinceId)
        .SelectMany(p => p.Cantons)
        .ToArrayAsync(cancellationToken);

        return cantons.Length > 0 ? cantons : Result<Canton[]>.NotFound("No cantons found");
    }

    public async Task<Result<District[]>> GetDistricts(int provinceId, int cantonId, CancellationToken cancellationToken)
    {
        var districts = await context.Cantons
        .Include(p => p.Districts)
        .Where(p => p.Id == cantonId && p.ProvinceId == provinceId)
        .SelectMany(p => p.Districts)
        .ToArrayAsync(cancellationToken);

        return districts.Length > 0 ? districts : Result<District[]>.NotFound("No districts found");
    }
}