using DiscoverCostaRica.Beaches.Domain.Interfaces;
using DiscoverCostaRica.Beaches.Domain.Models;
using DiscoverCostaRica.Beaches.Infraestructure.Interfaces;
using DiscoverCostaRica.Shared.Attributes;
using System.Data.Entity;

namespace DiscoverCostaRica.Beaches.Infraestructure.Services;

[TransientService]
public class BeachRepository(IBeachContext _context) : IBeachRepository
{
    public async Task<List<BeachModel>> GetBeaches(CancellationToken cancellationToken)
    {
        return await _context.Beaches.ToListAsync(cancellationToken);
    }
}
