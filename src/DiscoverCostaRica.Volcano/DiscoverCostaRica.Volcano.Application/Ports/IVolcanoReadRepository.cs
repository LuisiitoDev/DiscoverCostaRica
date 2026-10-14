namespace DiscoverCostaRica.Volcano.Application.Ports;

/// <summary>Read access to volcanoes. Lists come ordered by id.</summary>
public interface IVolcanoReadRepository
{
    Task<IReadOnlyList<Domain.Volcano>> GetAllAsync(CancellationToken cancellationToken);

    Task<Domain.Volcano?> FindAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Domain.Volcano>> GetByProvinceAsync(int provinceId, CancellationToken cancellationToken);
}
