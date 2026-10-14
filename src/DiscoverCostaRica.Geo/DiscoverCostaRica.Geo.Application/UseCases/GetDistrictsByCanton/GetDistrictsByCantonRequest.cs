namespace DiscoverCostaRica.Geo.Application.UseCases.GetDistrictsByCanton;

/// <param name="ProvinceId">
/// The canton's province. Null only for the deprecated v1 route, which then matches the canton
/// number in every province (GEO-D2).
/// </param>
public sealed record GetDistrictsByCantonRequest(int? ProvinceId, int CantonId);
