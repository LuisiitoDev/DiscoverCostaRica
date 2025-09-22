using DiscoverCostaRica.Beaches.Domain.Models;

namespace DiscoverCostaRica.Beaches.Domain.Interfaces;

public interface IBeachRepository
{
    Task<List<BeachModel>> GetBeaches(CancellationToken cancellationToken);
}
