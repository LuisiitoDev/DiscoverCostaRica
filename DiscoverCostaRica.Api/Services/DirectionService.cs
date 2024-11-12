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

    public async Task<Result<Canton[]>> GetCantons(CancellationToken cancellationToken)
    {
         return Result<Canton[]>.NotFound("No districts found.");
    }

    public async Task<Result<District[]>> GetDistricts(CancellationToken cancellationToken)
    {
        return Result<District[]>.NotFound("No districts found.");
    }
}