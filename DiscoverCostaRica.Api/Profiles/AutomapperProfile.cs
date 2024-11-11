using AutoMapper;
using DiscoverCostaRica.Api.Endpoints;
using DiscoverCostaRica.Api.Models.Dto;

namespace DiscoverCostaRica.Api.Profiles;
public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        // Beach mapping
        CreateMap<Beach, DtoBeach>().ReverseMap();
    }
}