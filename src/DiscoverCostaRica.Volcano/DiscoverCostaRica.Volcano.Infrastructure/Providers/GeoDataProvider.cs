using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Shared.Dtos;
using DiscoverCostaRica.Shared.Interfaces;
using DiscoverCostaRica.Volcano.Infrastructure.Interfaces;

namespace DiscoverCostaRica.Volcano.Infrastructure.Providers;

[TransientService]
public class GeoDataProvider(IGeoDiscoverCostaRica discoverCostaRica) : IGeoDataProvider
{
    public async Task<DtoCanton?> GetCantonById(int provinceId, int cantonId, CancellationToken cancellationToken)
    {
        var result = await discoverCostaRica.GetCanton(provinceId, cantonId, cancellationToken);
        return result.Value;
    }

    public async Task<DtoDistrict?> GetDistrictById(int cantonId, int? districtId, CancellationToken cancellationToken)
    {
        if (!districtId.HasValue) return null;

        var result = await discoverCostaRica.GetDistrict(cantonId, districtId.Value, cancellationToken);
        return result.Value;
    }

    public async Task<DtoProvince?> GetProvinceById(int id, CancellationToken cancellationToken)
    {
        var result = await discoverCostaRica.GetProvince(id, cancellationToken);
        return result.Value;
    }
}
