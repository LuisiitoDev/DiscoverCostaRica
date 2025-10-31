using DiscoverCostaRica.VolcanoService.Domain.Models;

namespace DiscoverCostaRica.VolcanoService.Domain.Interfaces;

public interface IVolcanoRepository
{
    Task<List<VolcanoModel>> GetVolcanos(CancellationToken cancellationToken);
    Task<VolcanoModel?> GetVolcanoById(int id, CancellationToken cancellationToken);
    Task<List<VolcanoModel>> GetVolcanosByProvince(int provinceId, CancellationToken cancellationToken);
}
