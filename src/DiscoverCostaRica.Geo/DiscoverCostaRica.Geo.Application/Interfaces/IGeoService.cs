using DiscoverCostaRica.Geo.Application.Dtos;
using DiscoverCostaRica.Shared.Responses;

namespace DiscoverCostaRica.Geo.Application.Interfaces;

public interface IGeoService
{
    Task<Result<List<ProvinceDto>>> GetProvinces(CancellationToken cancellationToken);
    Task<Result<List<CantonDto>>> GetCantons(int provinceId, CancellationToken cancellationToken);
    Task<Result<List<DistrictDto>>> GetDistricts(int cantonId, CancellationToken cancellationToken);
}

