using DiscoverCostaRica.Beaches.Domain;

namespace DiscoverCostaRica.Beaches.Application.Ports;

public interface IBeachReadRepository
{
    /// <summary>Every beach, ordered by id.</summary>
    Task<IReadOnlyList<Beach>> GetAllAsync(CancellationToken cancellationToken);
}
