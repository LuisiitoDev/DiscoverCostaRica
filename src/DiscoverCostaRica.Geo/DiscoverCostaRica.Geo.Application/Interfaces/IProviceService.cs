using DiscoverCostaRica.Geo.Application.Dtos;
using DiscoverCostaRica.Shared.Responses;

namespace DiscoverCostaRica.Geo.Application.Interfaces;

public interface IProviceService
{
    Task<Result<List<ProvinceDto>>> GetProvinces(CancellationToken cancellationToken);
    Task<Result<ProvinceDto>> GetProvinceById(int provinceId, CancellationToken cancellationToken);

}
