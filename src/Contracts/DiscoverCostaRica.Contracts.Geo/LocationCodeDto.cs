namespace DiscoverCostaRica.Contracts.Geo;

/// <summary>
/// Where something is, by official codes. Canton codes are unique only inside their province
/// and district codes only inside their canton, so all levels travel together.
/// </summary>
public sealed record LocationCodeDto(int ProvinceId, int CantonId, int? DistrictId = null);
