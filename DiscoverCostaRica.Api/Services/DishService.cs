using AutoMapper;
using DiscoverCostaRica.Api.Models;
using DiscoverCostaRica.Api.Models.Dto;
using DiscoverCostaRica.Infraestructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Api.Services;
public class DishService(DiscoverCostaRicaContext context, IMapper mapper)
{
    public async Task<Result<DtoDish[]>> GetDishesAsync(CancellationToken cancellationToken)
    {
        var dishes = await context.Dishes.ToArrayAsync(cancellationToken);
        return mapper.Map<DtoDish[]>(dishes);
    }
}