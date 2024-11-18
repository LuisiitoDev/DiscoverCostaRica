using AutoMapper;
using DiscoverCostaRica.Api.Models;
using DiscoverCostaRica.Api.Models.Dto;
using DiscoverCostaRica.Infraestructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Api.Services;

public class VolcanoService(DiscoverCostaRicaContext context, IMapper mapper)
{
    public async Task<Result<DtoVolcano[]>> GetVolcanos(CancellationToken cancellationToken)
    {
        var volcanos = await context.Volcanos.ToArrayAsync(cancellationToken);
        return volcanos.Length > 0 ? mapper.Map<DtoVolcano[]>(volcanos) : Result<DtoVolcano[]>.NotFound("Volcanos not found");
    }
}
