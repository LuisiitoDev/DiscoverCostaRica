using DiscoverCostaRica.Volcano.Domain.Dtos;
using DiscoverCostaRica.VolcanoService.Domain.Models;

namespace DiscoverCostaRica.Volcano.Application.Interfaces;

public interface IGeoClient
{
    Task<IDictionary<int, LocationDto>> GetVolcanoLocations(IEnumerable<VolcanoModel> volcanos, CancellationToken cancellationToken);
}
