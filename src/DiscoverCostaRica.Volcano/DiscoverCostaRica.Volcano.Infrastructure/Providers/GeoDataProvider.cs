using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Shared.Constants;
using DiscoverCostaRica.Shared.Interfaces;
using DiscoverCostaRica.Volcano.Domain.Dtos;
using DiscoverCostaRica.Volcano.Domain.Interfaces;

namespace DiscoverCostaRica.Volcano.Infrastructure.Providers;

[TransientService]
public class GeoDataProvider(IDiscoverCostaRicaDapr discoverCostaRica) : IGeoDataProvider
{
    public async Task<CantonDto?> GetCantonById(int provinceId, int cantonId, CancellationToken cancellationToken)
    {
        return await discoverCostaRica.InvokeGetMethodAsync<CantonDto>(
            Microservices.Geo,
            $"api/v1/geo/cantons/{provinceId}/{cantonId}",
            cancellationToken
        );
    }

    public async Task<DistrictDto?> GetDistrictById(int cantonId, int? districtId, CancellationToken cancellationToken)
    {
        if (!districtId.HasValue) return null;

        return await discoverCostaRica.InvokeGetMethodAsync<DistrictDto>(
            Microservices.Geo,
            $"api/v1/geo/districts/{cantonId}/{districtId.Value}",
            cancellationToken
        );
    }

    public async Task<ProvinceDto?> GetProvinceById(int id, CancellationToken cancellationToken)
    {
        return await discoverCostaRica.InvokeGetMethodAsync<ProvinceDto>(
            Microservices.Geo,
            $"api/v1/geo/provinces/{id}",
            cancellationToken
        );
    }
}
