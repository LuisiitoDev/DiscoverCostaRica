namespace DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection;

/// <summary>
/// Registers the class as a decorator of its first interface (e.g. a caching use case around
/// <c>IUseCase&lt;TRequest, TResponse&gt;</c>). Read at compile time by DiscoverCostaRica.SourceGenerators.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class DecoratorServiceAttribute : Attribute;
