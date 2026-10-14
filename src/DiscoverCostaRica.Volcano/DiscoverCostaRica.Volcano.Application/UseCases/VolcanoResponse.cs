namespace DiscoverCostaRica.Volcano.Application.UseCases;

/// <summary>An administrative division by code and name.</summary>
public sealed record NamedRef(int Id, string Name);

/// <summary>Where a volcano is, with names. A level is null when Geo does not know its code.</summary>
public sealed record LocationResponse(NamedRef? Province, NamedRef? Canton, NamedRef? District);

/// <summary>A volcano as every volcano use case returns it. <see cref="Location"/> is null when Geo could not be reached.</summary>
public sealed record VolcanoResponse(int Id, string Name, string Description, LocationResponse? Location);
