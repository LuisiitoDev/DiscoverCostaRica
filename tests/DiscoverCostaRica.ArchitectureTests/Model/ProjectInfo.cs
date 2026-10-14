namespace DiscoverCostaRica.ArchitectureTests.Model;

/// <param name="Name">Project (and assembly) name.</param>
/// <param name="Service">Owning microservice (Geo, Beaches, ...) or null for shared components.</param>
/// <param name="ProjectReferences">Referenced project names, excluding analyzer-only references.</param>
public sealed record ProjectInfo(
    string Name,
    string? Service,
    Layer Layer,
    IReadOnlyList<string> ProjectReferences,
    IReadOnlyList<string> PackageReferences);
