using AutoMapper;
using DiscoverCostaRica.Geo.Application.Dtos;
using DiscoverCostaRica.Geo.Application.Interfaces;
using DiscoverCostaRica.Geo.Domain.Interfaces;
using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Shared.Responses;
using Microsoft.AspNetCore.Http;

namespace DiscoverCostaRica.Geo.Application.Services;

[TransientService]
public class ProvinceService(IGeoRepository repository, IMapper mapper) : IProvinceService
{
    public async Task<Result<ProvinceDto>> GetProvinceById(int provinceId, CancellationToken cancellationToken)
    {
        var province = await repository.GetProvinceById(provinceId, cancellationToken);

        if (province is null) return new Failure($"No provice was found for {provinceId}", StatusCodes.Status404NotFound);

        return new Success(mapper.Map<ProvinceDto>(province));
    }

    public async Task<Result<List<ProvinceDto>>> GetProvinces(CancellationToken cancellationToken)
    {
        var provinces = await repository.GetProvinces(cancellationToken);

        if (provinces is null || provinces.Count <= 0)
            return new Failure("No provinces found. Please check later for updates.", StatusCodes.Status404NotFound);

        return new Success(mapper.Map<List<ProvinceDto>>(provinces));
    }
}
