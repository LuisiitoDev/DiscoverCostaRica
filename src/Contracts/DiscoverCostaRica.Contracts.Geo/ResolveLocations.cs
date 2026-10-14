namespace DiscoverCostaRica.Contracts.Geo;

/// <summary>Body of <see cref="GeoContract.ResolveLocationsRoute"/>: 1 to <see cref="GeoContract.MaxResolveBatchSize"/> codes.</summary>
public sealed record ResolveLocationsRequest(IReadOnlyList<LocationCodeDto> Items);

/// <summary>Resolved names in the same order and with the same length as the request.</summary>
public sealed record ResolveLocationsResponse(IReadOnlyList<ResolvedLocationDto> Items);
