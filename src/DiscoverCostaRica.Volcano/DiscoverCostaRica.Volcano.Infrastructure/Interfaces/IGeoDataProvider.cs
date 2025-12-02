using DiscoverCostaRica.Shared.Dtos;

namespace DiscoverCostaRica.Volcano.Infrastructure.Interfaces;

public interface IGeoDataProvider
{
    Task<DtoCanton?> GetCantonById(int provinceId, int cantonId, CancellationToken cancellationToken);
    Task<DtoDistrict?> GetDistrictById(int cantonId, int? districtId, CancellationToken cancellationToken);
    Task<DtoProvince?> GetProvinceById(int id, CancellationToken cancellationToken);
}
