namespace DiscoverCostaRica.Geo.Application.UseCases.ResolveLocations;

/// <summary>Codes of one location. Canton and district numbers are only unique inside their parent.</summary>
public sealed record LocationQuery(int ProvinceId, int CantonId, int? DistrictId);

public sealed record ResolveLocationsRequest(IReadOnlyList<LocationQuery>? Items);
