using DiscoverCostaRica.Volcano.Application.UseCases;
using DiscoverCostaRica.Volcano.Domain;

namespace DiscoverCostaRica.Volcano.Application.Ports;

/// <summary>
/// Names for location codes, owned by the Geo service. Implementations must not fail the caller:
/// a code that cannot be resolved (unknown, or Geo unavailable) is simply absent from the result.
/// </summary>
public interface IGeoLocationGateway
{
    Task<IReadOnlyDictionary<LocationCode, LocationResponse>> ResolveAsync(
        IReadOnlyCollection<LocationCode> codes,
        CancellationToken cancellationToken);
}
