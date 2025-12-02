using DiscoverCostaRica.Volcano.Application.Dtos;
using DiscoverCostaRica.VolcanoService.Domain.Models;

namespace DiscoverCostaRica.Volcano.Application.Interfaces;

public interface ILocationService
{
    Task<IDictionary<int, DtoLocation>> GetVolcanoLocations(IEnumerable<VolcanoModel> volcanos, CancellationToken cancellationToken);
}
