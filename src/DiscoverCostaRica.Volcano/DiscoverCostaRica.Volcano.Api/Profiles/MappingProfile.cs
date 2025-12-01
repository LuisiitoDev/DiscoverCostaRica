using AutoMapper;
using DiscoverCostaRica.Volcano.Domain.Dtos;
using DiscoverCostaRica.VolcanoService.Domain.Models;

namespace DiscoverCostaRica.VolcanoService.Api.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<VolcanoModel, VolcanoDto>();
    }
}
