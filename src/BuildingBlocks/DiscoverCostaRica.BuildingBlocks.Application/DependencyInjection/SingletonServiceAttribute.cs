namespace DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection;

/// <summary>
/// Registers the class as a singleton service for its first interface.
/// Read at compile time by DiscoverCostaRica.SourceGenerators; carries no runtime dependency.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class SingletonServiceAttribute : Attribute;
