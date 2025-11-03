using AutoMapper;
using DiscoverCostaRica.Geo.Application.Dtos;
using DiscoverCostaRica.Geo.Application.Interfaces;
using DiscoverCostaRica.Geo.Domain.Interfaces;
using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DiscoverCostaRica.Geo.Application.Services;

[TransientService]
public class GeoService(IGeoRepository repository, IMapper mapper, ILogger<GeoService> logger) : IGeoService
{
    public async Task<Result<List<CantonDto>>> GetCantons(int provinceId, CancellationToken cancellationToken)
    {
        try
        {
            var cantons = await repository.GetCantonsByProvince(provinceId, cancellationToken);

            if (cantons is null || cantons.Count <= 0)
                return new Failure("No cantons found. Please check later for updates.", StatusCodes.Status404NotFound);

            return new Success(mapper.Map<List<CantonDto>>(cantons));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandle exception");
            return new Failure("Unhandle exception", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<Result<List<DistrictDto>>> GetDistricts(int cantonId, CancellationToken cancellationToken)
    {
        try
        {
            var districts = await repository.GetDistrictsByCanton(cantonId, cancellationToken);

            if (districts is null || districts.Count <= 0)
                return new Failure("No districts found. Please check later for updates.", StatusCodes.Status404NotFound);

            return new Success(mapper.Map<List<DistrictDto>>(districts));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandle exception");
            return new Failure("Unhandle exception", StatusCodes.Status500InternalServerError);
        }

    }

    public async Task<Result<List<ProvinceDto>>> GetProvinces(CancellationToken cancellationToken)
    {
        try
        {
            var provinces = await repository.GetProvinces(cancellationToken);

            if (provinces is null || provinces.Count <= 0)
                return new Failure("No provinces found. Please check later for updates.", StatusCodes.Status404NotFound);

            return new Success(mapper.Map<List<ProvinceDto>>(provinces));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandle exception");
            return new Failure("Unhandle exception", StatusCodes.Status500InternalServerError);
        }
    }
}
