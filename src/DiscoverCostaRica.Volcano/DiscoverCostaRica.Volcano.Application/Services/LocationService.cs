using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Volcano.Application.Dtos;
using DiscoverCostaRica.Volcano.Application.Interfaces;
using DiscoverCostaRica.Volcano.Infrastructure.Interfaces;
using DiscoverCostaRica.VolcanoService.Domain.Models;
using static DiscoverCostaRica.Shared.Routes.RoutesConstants;

namespace DiscoverCostaRica.Volcano.Application.Services;

[TransientService]
public class LocationService(IGeoDataProvider provider) : ILocationService
{
    public async Task<IDictionary<int, DtoLocation>> GetVolcanoLocations(IEnumerable<VolcanoModel> volcanos, CancellationToken cancellationToken)
    {
        //await provider.GetProvinceById(1, cancellationToken);
        // This part is fine: it runs one GetLocation task for every volcano concurrently.
        var tasks = volcanos.Select(volcano => GetLocation(volcano, cancellationToken));
        var locations = await Task.WhenAll(tasks);

        return volcanos.Zip(locations, (volcano, location) => (volcano, location))
                        .ToDictionary(pair => pair.volcano.Id, pair => pair.location);
    }

    private async Task<DtoLocation> GetLocation(VolcanoModel volcano, CancellationToken cancellationToken)
    {
        // 🚀 FIX: Start all three network calls simultaneously
        var provinceTask = provider.GetProvinceById(volcano.ProvinceId, cancellationToken);
        var cantonTask = provider.GetCantonById(volcano.ProvinceId, volcano.CantonId, cancellationToken);
        var districtTask = provider.GetDistrictById(volcano.CantonId, volcano.DistrictId, cancellationToken);

        // Wait for all three concurrent tasks to complete
        await Task.WhenAll(provinceTask, cantonTask, districtTask);

        // Read the results from the completed tasks
        return new DtoLocation
        {
            Province = await provinceTask,
            Canton = await cantonTask,
            District = await districtTask
        };
    }
}
