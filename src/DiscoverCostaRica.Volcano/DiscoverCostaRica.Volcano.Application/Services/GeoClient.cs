using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Volcano.Application.Interfaces;
using DiscoverCostaRica.Volcano.Domain.Dtos;
using DiscoverCostaRica.Volcano.Domain.Interfaces;
using DiscoverCostaRica.VolcanoService.Domain.Models;

namespace DiscoverCostaRica.Volcano.Application.Services;

[TransientService]
public class GeoClient(IGeoDataProvider provider) : IGeoClient
{
    public async Task<IDictionary<int, LocationDto>> GetVolcanoLocations(IEnumerable<VolcanoModel> volcanos, CancellationToken cancellationToken)
    {
        var tasks = volcanos.Select(volcano => GetLocation(volcano, cancellationToken));
        var locations = await Task.WhenAll(tasks);
        return volcanos.Zip(locations, (volcano, location) => (volcano, location))
                       .ToDictionary(pair => pair.volcano.Id, pair => pair.location);
    }

    private async Task<LocationDto> GetLocation(VolcanoModel volcano, CancellationToken cancellationToken)
    {
        return new LocationDto
        {
            Province = await provider.GetProvinceById(volcano.ProvinceId, cancellationToken),
            Canton = await provider.GetCantonById(volcano.ProvinceId, volcano.CantonId, cancellationToken),
            District = await provider.GetDistrictById(volcano.CantonId, volcano.DistrictId, cancellationToken)
        };
    }
}
