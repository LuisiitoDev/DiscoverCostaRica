using DiscoverCostaRica.Culture.Domain.Interfaces;
using DiscoverCostaRica.Culture.Domain.Models;
using DiscoverCostaRica.Culture.Infraestructure.Interfaces;
using DiscoverCostaRica.Shared.Attributes;
using System.Data.Entity;

namespace DiscoverCostaRica.Culture.Infraestructure.Services;

[TransientService]
public class CultureRepository(ICultureContext _context) : ICultureRepository
{
    public Task<List<DishModel>> GetDishes(CancellationToken cancellationToken)
    {
        return _context.Dishes.ToListAsync(cancellationToken);
    }

    public Task<List<TraditionModel>> GetTraditions(CancellationToken cancellationToken)
    {
        return _context.Traditions.ToListAsync(cancellationToken);
    }
}
