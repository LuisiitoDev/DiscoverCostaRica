using DiscoverCostaRica.Volcano.Domain.Dtos;

namespace DiscoverCostaRica.Volcano.Domain.Interfaces;

public interface IGeoDataProvider
{
    Task<ProvinceDto?> GetProvinceById(int id, CancellationToken cancellationToken);
    Task<CantonDto?> GetCantonById(int provinceId, int cantonId, CancellationToken cancellationToken);
    Task<DistrictDto?> GetDistrictById(int cantonId, int? districtId, CancellationToken cancellationToken);
}
