using DiscoverCostaRica.Volcano.Application.Dtos;
using DiscoverCostaRica.VolcanoService.Domain.Models;

namespace DiscoverCostaRica.Volcano.Application.Interfaces;

public interface IMapperVolcanoService
{
    IEnumerable<DtoVolcano> MapVolcanoesWithLocations(IEnumerable<VolcanoModel> volcanos, IDictionary<int, DtoLocation> locations);
    IEnumerable<DtoVolcano> MapVolcanos(IEnumerable<VolcanoModel> volcanos);
    DtoVolcano MapVolcano(VolcanoModel volcano);
}
