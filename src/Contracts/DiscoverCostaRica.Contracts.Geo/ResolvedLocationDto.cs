namespace DiscoverCostaRica.Contracts.Geo;

/// <summary>Names for one <see cref="LocationCodeDto"/>. A level is null when its code is unknown.</summary>
public sealed record ResolvedLocationDto(NamedRefDto? Province, NamedRefDto? Canton, NamedRefDto? District);
