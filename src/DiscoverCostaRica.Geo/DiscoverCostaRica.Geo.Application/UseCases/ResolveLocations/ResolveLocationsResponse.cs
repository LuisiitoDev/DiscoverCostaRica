namespace DiscoverCostaRica.Geo.Application.UseCases.ResolveLocations;

/// <summary>Names for one queried location. A level is null when its code is unknown or invalid.</summary>
public sealed record ResolvedLocation(DivisionResponse? Province, DivisionResponse? Canton, DivisionResponse? District);

/// <summary>One entry per queried item, in the same order.</summary>
public sealed record ResolveLocationsResponse(IReadOnlyList<ResolvedLocation> Items);
