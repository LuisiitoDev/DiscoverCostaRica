using AutoMapper;
using DiscoverCostaRica.Geo.Application.Dtos;
using DiscoverCostaRica.Geo.Application.Interfaces;
using DiscoverCostaRica.Geo.Domain.Interfaces;
using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Shared.Responses;
using Microsoft.AspNetCore.Http;

namespace DiscoverCostaRica.Geo.Application.Services;

[TransientService]
public class GeoService(IGeoRepository _repository, IMapper _mapper) : IGeoService
{
    public async Task<Result<List<CantonDto>>> GetCantons(int provinceId, CancellationToken cancellationToken)
    {
        var cantons = await _repository.GetCantonsByProvince(provinceId, cancellationToken);

        if (cantons is null || cantons.Count <= 0)
            return new Failure("No cantons found. Please check later for updates.", StatusCodes.Status404NotFound);

        return new Success(_mapper.Map<List<CantonDto>>(cantons));
    }

    public async Task<Result<List<DistrictDto>>> GetDistricts(int cantonId, CancellationToken cancellationToken)
    {
        var cantons = await _repository.GetDistrictsByCanton(cantonId, cancellationToken);

        if (cantons is null || cantons.Count <= 0)
            return new Failure("No districts found. Please check later for updates.", StatusCodes.Status404NotFound);

        return new Success(_mapper.Map<List<DistrictDto>>(cantons));
    }

    public async Task<Result<List<ProvinceDto>>> GetProvinces(CancellationToken cancellationToken)
    {
        var provinces = await _repository.GetProvinces(cancellationToken);

        if (provinces is null || provinces.Count <= 0)
            return new Failure("No provinces found. Please check later for updates.", StatusCodes.Status404NotFound);

        return new Success(_mapper.Map<List<ProvinceDto>>(provinces));
    }
}
