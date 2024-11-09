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

    public async Task<Result<Canton[]>> GetCantons(CancellationToken cancellationToken)
    {
        var cantons = await context.Cantons.ToArrayAsync(cancellationToken);
        return cantons.Length > 0 ? cantons : Result<Canton[]>.NotFound("No cantons found.");
    }

    public async Task<Result<District[]>> GetDistricts(CancellationToken cancellationToken)
    {
        var districts = await context.Districts.ToArrayAsync(cancellationToken);
        return districts.Length > 0 ? districts : Result<District[]>.NotFound("No districts found.");
    }

    public async Task<Result<ProvinceDetail>> GetProvinceDetail(short provinceId, CancellationToken cancellationToken)
    {
        return await context.ProvinceDetails.FirstAsync(p => p.ProvinceId == provinceId, cancellationToken);
    }
}