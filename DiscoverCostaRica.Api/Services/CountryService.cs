using AutoMapper;
using DiscoverCostaRica.Api.Models;
using DiscoverCostaRica.Api.Models.Dto;
using DiscoverCostaRica.Infraestructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Api.Services;

public class CountryService(DiscoverCostaRicaContext context, IMapper mapper)
{
    public async Task<Result<DtoCountry>> GetCountry(int countryCode, CancellationToken cancellationToken)
    {
        var country =await context.Countries
            .Include(c => c.Capital)
            .SingleOrDefaultAsync(c => c.CountryCode == countryCode, cancellationToken);

        return country is null ? Result<DtoCountry>.NotFound("Country was not found") :
            mapper.Map<DtoCountry>(country);
    }
}
