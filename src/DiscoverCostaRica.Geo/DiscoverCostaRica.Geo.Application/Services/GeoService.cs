using AutoMapper;
using DiscoverCostaRica.Geo.Application.Dtos;
using DiscoverCostaRica.Geo.Application.Interfaces;
using DiscoverCostaRica.Geo.Domain.Interfaces;
using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DiscoverCostaRica.Geo.Application.Services;

/// <summary>
/// Provides geographic data retrieval services for provinces, cantons, and districts.
/// </summary>
[TransientService]
public class GeoService(IGeoRepository repository, IMapper mapper, ILogger<GeoService> logger) : IGeoService
{
    /// <summary>
    /// Retrieves a province by its identifier.
    /// </summary>
    /// <param name="provinceId">The unique identifier of the province.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Result{ProvinceDto}"/> containing the province data if found; otherwise, a failure result.
    /// </returns>
    public async Task<Result<ProvinceDto>> GetProvinceById(int provinceId, CancellationToken cancellationToken)
    {
        var province = await repository.GetProvinceById(provinceId, cancellationToken);

        if (province is null) return new Failure($"No provice was found for {provinceId}", StatusCodes.Status404NotFound);

        return new Success(mapper.Map<ProvinceDto>(province));
    }

    /// <summary>
    /// Retrieves a canton by its province and canton identifiers.
    /// </summary>
    /// <param name="provinceId">The unique identifier of the province.</param>
    /// <param name="cantonId">The unique identifier of the canton.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Result{CantonDto}"/> containing the canton data if found; otherwise, a failure result.
    /// </returns>
    public async Task<Result<CantonDto>> GetCantonById(int provinceId, int cantonId, CancellationToken cancellationToken)
    {
        var canton = await repository.GetCantonById(provinceId, cantonId, cancellationToken);

        if (canton is null) return new Failure($"No canton was found for {cantonId}", StatusCodes.Status404NotFound);

        return new Success(mapper.Map<CantonDto>(canton));
    }

    /// <summary>
    /// Retrieves a district by its canton and district identifiers.
    /// </summary>
    /// <param name="cantonId">The unique identifier of the canton.</param>
    /// <param name="districtId">The unique identifier of the district.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Result{DistrictDto}"/> containing the district data if found; otherwise, a failure result.
    /// </returns>
    public async Task<Result<DistrictDto>> GetDistrictById(int cantonId, int districtId, CancellationToken cancellationToken)
    {
        var canton = await repository.GetDistrictById(cantonId, districtId, cancellationToken);

        if (canton is null) return new Failure($"No district was found for {districtId}", StatusCodes.Status404NotFound);

        return new Success(mapper.Map<DistrictDto>(canton));
    }

    /// <summary>
    /// Retrieves all cantons for a given province.
    /// </summary>
    /// <param name="provinceId">The unique identifier of the province.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Result{List{CantonDto}}"/> containing the list of cantons if found; otherwise, a failure result.
    /// </returns>
    public async Task<Result<List<CantonDto>>> GetCantons(int provinceId, CancellationToken cancellationToken)
    {
        var cantons = await repository.GetCantonsByProvince(provinceId, cancellationToken);

        if (cantons is null || cantons.Count <= 0)
            return new Failure("No cantons found. Please check later for updates.", StatusCodes.Status404NotFound);

        return new Success(mapper.Map<List<CantonDto>>(cantons));
    }

    /// <summary>
    /// Retrieves all districts for a given canton.
    /// </summary>
    /// <param name="cantonId">The unique identifier of the canton.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Result{List{DistrictDto}}"/> containing the list of districts if found; otherwise, a failure result.
    /// </returns>
    public async Task<Result<List<DistrictDto>>> GetDistricts(int cantonId, CancellationToken cancellationToken)
    {
        var districts = await repository.GetDistrictsByCanton(cantonId, cancellationToken);

        if (districts is null || districts.Count <= 0)
            return new Failure("No districts found. Please check later for updates.", StatusCodes.Status404NotFound);

        return new Success(mapper.Map<List<DistrictDto>>(districts));
    }

    /// <summary>
    /// Retrieves all provinces.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Result{List{ProvinceDto}}"/> containing the list of provinces if found; otherwise, a failure result.
    /// </returns>
    public async Task<Result<List<ProvinceDto>>> GetProvinces(CancellationToken cancellationToken)
    {
        var provinces = await repository.GetProvinces(cancellationToken);

        if (provinces is null || provinces.Count <= 0)
            return new Failure("No provinces found. Please check later for updates.", StatusCodes.Status404NotFound);

        return new Success(mapper.Map<List<ProvinceDto>>(provinces));
    }
}
