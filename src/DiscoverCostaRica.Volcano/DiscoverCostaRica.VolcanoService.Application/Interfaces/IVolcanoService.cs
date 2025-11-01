using DiscoverCostaRica.Shared.Responses;
using DiscoverCostaRica.VolcanoService.Application.Dtos;

namespace DiscoverCostaRica.VolcanoService.Application.Interfaces;

public interface IVolcanoService
{
    Task<Result<List<VolcanoDto>>> GetVolcanos(CancellationToken cancellationToken);
    Task<Result<List<VolcanoDto>>> GetVolcanosByProvince(int provinceId, CancellationToken cancellationToken);
    Task<Result<VolcanoDto>> GetVolcanoById(int id, CancellationToken cancellationToken);
}
