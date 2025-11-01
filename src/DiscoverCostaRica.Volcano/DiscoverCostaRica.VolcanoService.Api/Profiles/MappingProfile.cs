using AutoMapper;
using DiscoverCostaRica.VolcanoService.Application.Dtos;
using DiscoverCostaRica.VolcanoService.Domain.Models;

namespace DiscoverCostaRica.VolcanoService.Api.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<VolcanoModel, VolcanoDto>();
    }
}
