using AutoMapper;
using DiscoverCostaRica.Api.Models.Dto;
using DiscoverCostaRica.Domain.Entities;

namespace DiscoverCostaRica.Api.Profiles;
public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        CreateMap<Beach, DtoBeach>().ReverseMap();
        CreateMap<Dish, DtoDish>().ReverseMap();
        CreateMap<Volcano, DtoVolcano>().ReverseMap();
        CreateMap<Province, DtoProvince>().ReverseMap();
        CreateMap<Canton, DtoCanton>().ReverseMap();
        CreateMap<District, DtoDistrict>().ReverseMap();
    }
}