namespace DiscoverCostaRica.Contracts.Geo;

/// <summary>An administrative division reduced to what consumers display: its code and name.</summary>
public sealed record NamedRefDto(int Id, string Name);
