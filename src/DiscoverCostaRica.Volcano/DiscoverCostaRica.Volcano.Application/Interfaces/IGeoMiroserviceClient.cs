using DiscoverCostaRica.Volcano.Application.Dtos;
using DiscoverCostaRica.VolcanoService.Domain.Models;

namespace DiscoverCostaRica.Volcano.Application.Interfaces;

public interface IGeoMiroserviceClient
{
    Task<IDictionary<VolcanoModel, LocationDto>> GetVolcanoLocations(IEnumerable<VolcanoModel> volcanos, CancellationToken cancellationToken);
}
