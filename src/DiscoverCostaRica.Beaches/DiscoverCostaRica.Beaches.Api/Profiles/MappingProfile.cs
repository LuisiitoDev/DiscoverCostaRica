using AutoMapper;
using DiscoverCostaRica.Beaches.Application.Dtos;
using DiscoverCostaRica.Beaches.Domain.Models;

namespace DiscoverCostaRica.Beaches.Api.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<BeachModel, DtoBeach>().ReverseMap();
    }
}
