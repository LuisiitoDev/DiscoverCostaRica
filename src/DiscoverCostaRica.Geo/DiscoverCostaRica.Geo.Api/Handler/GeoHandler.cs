using DiscoverCostaRica.Geo.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverCostaRica.Geo.Api.Handler;

/// <summary>
/// Provides handler methods for geographic data operations.
/// </summary>
public static class GeoHandler
{
    /// <summary>
    /// Retrieves a province by its identifier.
    /// </summary>
    /// <param name="provinceId">The identifier of the province.</param>
    /// <param name="service">The geographic service instance.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The result containing the province data.</returns>
    public static async Task<IResult> GetProvinceById(int provinceId, IProvinceService service, CancellationToken cancellationToken)
    {
        var result = await service.GetProvinceById(provinceId, cancellationToken);
        return result.ToResult();
    }

    /// <summary>
    /// Retrieves a canton by its province and canton identifiers.
    /// </summary>
    /// <param name="provinceId">The identifier of the province.</param>
    /// <param name="cantonId">The identifier of the canton.</param>
    /// <param name="service">The geographic service instance.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The result containing the canton data.</returns>
    public static async Task<IResult> GetCantonById(int provinceId, int cantonId, IGeoService service, CancellationToken cancellationToken)
    {
        var result = await service.GetCantonById(provinceId, cantonId, cancellationToken);
        return result.ToResult();
    }

    /// <summary>
    /// Retrieves a district by its canton and district identifiers.
    /// </summary>
    /// <param name="cantonId">The identifier of the canton.</param>
    /// <param name="districtId">The identifier of the district.</param>
    /// <param name="service">The geographic service instance.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The result containing the district data.</returns>
    public static async Task<IResult> GetDistrictById(int cantonId, int districtId, IGeoService service, CancellationToken cancellationToken)
    {
        var result = await service.GetDistrictById(cantonId, districtId, cancellationToken);
        return result.ToResult();
    }

    /// <summary>
    /// Retrieves all provinces.
    /// </summary>
    /// <param name="service">The geographic service instance.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The result containing the list of provinces.</returns>
    public static async Task<IResult> GetProvinces([FromServices] IProvinceService service, CancellationToken cancellationToken)
    {
        var result = await service.GetProvinces(cancellationToken);
        return result.ToResult();
    }

    /// <summary>
    /// Retrieves all cantons for a given province.
    /// </summary>
    /// <param name="provinceId">The identifier of the province.</param>
    /// <param name="service">The geographic service instance.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The result containing the list of cantons.</returns>
    public static async Task<IResult> GetCantonsByProvince(int provinceId, IGeoService service, CancellationToken cancellationToken)
    {
        var result = await service.GetCantons(provinceId, cancellationToken);
        return result.ToResult();
    }

    /// <summary>
    /// Retrieves all districts for a given canton.
    /// </summary>
    /// <param name="cantonId">The identifier of the canton.</param>
    /// <param name="service">The geographic service instance.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The result containing the list of districts.</returns>
    public static async Task<IResult> GetDistrictsByCanton(int cantonId, IGeoService service, CancellationToken cancellationToken)
    {
        var result = await service.GetDistricts(cantonId, cancellationToken);
        return result.ToResult();
    }
}
