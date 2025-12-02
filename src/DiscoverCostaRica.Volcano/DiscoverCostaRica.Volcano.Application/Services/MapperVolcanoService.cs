using AutoMapper;
using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Volcano.Application.Dtos;
using DiscoverCostaRica.Volcano.Application.Interfaces;
using DiscoverCostaRica.VolcanoService.Domain.Models;

namespace DiscoverCostaRica.Volcano.Application.Services;

[SingletonService]
public class MapperVolcanoService(IMapper mapper) : IMapperVolcanoService
{
    public IEnumerable<DtoVolcano> MapVolcanoesWithLocations(IEnumerable<VolcanoModel> volcanos, IDictionary<int, DtoLocation> locations)
    {
        return volcanos.Select(volcano => new DtoVolcano
        {
            Id = volcano.Id,
            Name = volcano.Name,
            Description = volcano.Description,
            Location = locations[volcano.Id]
        });
    }

    public IEnumerable<DtoVolcano> MapVolcanos(IEnumerable<VolcanoModel> volcanos)
    {
        return mapper.Map<IEnumerable<DtoVolcano>>(volcanos);
    }

    public DtoVolcano MapVolcano(VolcanoModel volcano)
    {
        return mapper.Map<DtoVolcano>(volcano);
    }
}
