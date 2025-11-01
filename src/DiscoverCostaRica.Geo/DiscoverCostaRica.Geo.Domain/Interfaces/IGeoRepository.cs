using DiscoverCostaRica.Geo.Domain.Models;

namespace DiscoverCostaRica.Geo.Domain.Interfaces;

public interface IGeoRepository
{
    Task<List<ProvinceModel>> GetProvinces(CancellationToken cancellationToken);
    Task<List<CantonModel>> GetCantonsByProvince(int provinceId, CancellationToken cancellationToken);
    Task<List<DistrictModel>> GetDistrictsByCanton(int cantonId, CancellationToken cancellationToken);
}
