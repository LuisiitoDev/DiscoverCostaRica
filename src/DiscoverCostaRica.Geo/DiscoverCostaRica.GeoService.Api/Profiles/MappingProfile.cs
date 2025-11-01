using AutoMapper;
using DiscoverCostaRica.Geo.Application.Dtos;
using DiscoverCostaRica.Geo.Domain.Models;

namespace DiscoverCostaRica.Geo.Api.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ProvinceDto, ProvinceModel>().ReverseMap();
        CreateMap<CantonDto, CantonModel>().ReverseMap();
        CreateMap<DistrictDto, DistrictModel>().ReverseMap();
    }
}
