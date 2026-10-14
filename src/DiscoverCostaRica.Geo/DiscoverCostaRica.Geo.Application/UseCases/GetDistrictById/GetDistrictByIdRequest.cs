namespace DiscoverCostaRica.Geo.Application.UseCases.GetDistrictById;

/// <param name="ProvinceId">
/// The canton's province. Null only for the deprecated v1 route, which then takes the lowest
/// province that has this canton and district number (GEO-D3).
/// </param>
public sealed record GetDistrictByIdRequest(int? ProvinceId, int CantonId, int DistrictId);
