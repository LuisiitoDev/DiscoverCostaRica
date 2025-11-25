using DiscoverCostaRica.Geo.Domain.Models;

namespace DiscoverCostaRica.Geo.Domain.Interfaces;

public interface IGeoRepository
{
    Task<ProvinceModel?> GetProvinceById(int provinceId, CancellationToken cancellationToken);
    Task<CantonModel?> GetCantonById(int provinceId, int cantonId, CancellationToken cancellationToken);
    Task<DistrictModel?> GetDistrictById(int cantonId, int districtId, CancellationToken cancellationToken);
    Task<List<ProvinceModel>> GetProvinces(CancellationToken cancellationToken);
    Task<List<CantonModel>> GetCantonsByProvince(int provinceId, CancellationToken cancellationToken);
    Task<List<DistrictModel>> GetDistrictsByCanton(int cantonId, CancellationToken cancellationToken);
}
