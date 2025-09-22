using DiscoverCostaRica.Beaches.Domain.Interfaces;
using DiscoverCostaRica.Beaches.Domain.Models;
using DiscoverCostaRica.Beaches.Infraestructure.Context;
using System.Data.Entity;

namespace DiscoverCostaRica.Beaches.Infraestructure.Services;

public class BeachRepository(BeachContext _context) : IBeachRepository
{
    public async Task<List<BeachModel>> GetBeaches(CancellationToken cancellationToken)
    {
        return await _context.Beaches.ToListAsync(cancellationToken);
    }
}
