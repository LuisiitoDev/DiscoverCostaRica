using DiscoverCostaRica.Shared.Responses;
using DiscoverCostaRica.Volcano.Domain.Dtos;

namespace DiscoverCostaRica.VolcanoService.Application.Interfaces;

public interface IVolcanoService
{
    Task<Result<IEnumerable<VolcanoDto>>> GetVolcanos(CancellationToken cancellationToken);
    Task<Result<List<VolcanoDto>>> GetVolcanosByProvince(int provinceId, CancellationToken cancellationToken);
    Task<Result<VolcanoDto>> GetVolcanoById(int id, CancellationToken cancellationToken);
}
