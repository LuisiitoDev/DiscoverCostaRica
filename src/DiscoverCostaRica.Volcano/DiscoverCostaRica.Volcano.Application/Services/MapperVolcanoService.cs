using AutoMapper;
using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Volcano.Application.Interfaces;
using DiscoverCostaRica.Volcano.Domain.Dtos;
using DiscoverCostaRica.VolcanoService.Domain.Models;

namespace DiscoverCostaRica.Volcano.Application.Services;

[SingletonService]
public class MapperVolcanoService(IMapper mapper) : IMapperVolcanoService
{
    public IEnumerable<VolcanoDto> MapVolcanoesWithLocations(IEnumerable<VolcanoModel> volcanos, IDictionary<int, LocationDto> locations)
    {
        return volcanos.Select(volcano => new VolcanoDto
        {
            Id = volcano.Id,
            Name = volcano.Name,
            Description = volcano.Description,
            Location = locations[volcano.Id]
        });
    }

    public IEnumerable<VolcanoDto> MapVolcanos(IEnumerable<VolcanoModel> volcanos)
    {
        return mapper.Map<IEnumerable<VolcanoDto>>(volcanos);
    }

    public VolcanoDto MapVolcano(VolcanoModel volcano)
    {
        return mapper.Map<VolcanoDto>(volcano);
    }
}
