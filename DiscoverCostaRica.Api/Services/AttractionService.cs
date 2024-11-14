using AutoMapper;
using DiscoverCostaRica.Api.Models;
using DiscoverCostaRica.Api.Models.Dto;
using DiscoverCostaRica.Infraestructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Api.Services;

public class AttractionService(DiscoverCostaRicaContext context, IMapper mapper)
{
    public async Task<Result<DtoAttraction[]>> GetAttractions(int provinceId,CancellationToken cancellationToken)
    {
        var attractions = await context
            .Provinces.Include(p => p.Attractions)
            .Where(p => p.Id == provinceId)
            .SelectMany(p => p.Attractions)
            .ToListAsync(cancellationToken);

        return mapper.Map<DtoAttraction[]>(attractions);
    }
}
