using AutoMapper;
using DiscoverCostaRica.Api.Models.Dto;
using DiscoverCostaRica.Domain.Entities;

namespace DiscoverCostaRica.Api.Profiles;
public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        // Beach mapping
        CreateMap<Beach, DtoBeach>().ReverseMap();
        // Dish mapping
        CreateMap<Dish, DtoDish>().ReverseMap();
    }
}