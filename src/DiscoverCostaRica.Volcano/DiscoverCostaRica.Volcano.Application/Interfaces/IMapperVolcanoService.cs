using DiscoverCostaRica.Volcano.Domain.Dtos;
using DiscoverCostaRica.VolcanoService.Domain.Models;

namespace DiscoverCostaRica.Volcano.Application.Interfaces;

public interface IMapperVolcanoService
{
    IEnumerable<VolcanoDto> MapVolcanoesWithLocations(IEnumerable<VolcanoModel> volcanos, IDictionary<int, LocationDto> locations);
    IEnumerable<VolcanoDto> MapVolcanos(IEnumerable<VolcanoModel> volcanos);
    VolcanoDto MapVolcano(VolcanoModel volcano);
}
