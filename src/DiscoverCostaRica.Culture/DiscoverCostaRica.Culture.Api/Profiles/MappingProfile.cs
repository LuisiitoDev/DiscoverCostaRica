using AutoMapper;
using DiscoverCostaRica.Culture.Application.Dtos;
using DiscoverCostaRica.Culture.Domain.Models;

namespace DiscoverCostaRica.Culture.Api.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<DishModel, DtoDish>().ReverseMap();
        CreateMap<TraditionModel, DtoTradition>().ReverseMap();
    }
}
