using DiscoverCostaRica.Shared.Responses;
using DiscoverCostaRica.Volcano.Application.Dtos;

namespace DiscoverCostaRica.VolcanoService.Application.Interfaces;

public interface IVolcanoService
{
    Task<Result<IEnumerable<DtoVolcano>>> GetVolcanos(CancellationToken cancellationToken);
    Task<Result<List<DtoVolcano>>> GetVolcanosByProvince(int provinceId, CancellationToken cancellationToken);
    Task<Result<DtoVolcano>> GetVolcanoById(int id, CancellationToken cancellationToken);
}
