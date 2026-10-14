using DiscoverCostaRica.BuildingBlocks.Infrastructure.Authentication;
using DiscoverCostaRica.Testing.Authentication;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DiscoverCostaRica.CharacterizationTests.Harness;

/// <summary>
/// The only place that knows how the services reach remote infrastructure.
/// When the wiring changes, this file changes; the tests do not.
/// </summary>
internal static class InfrastructureSeams
{
    public static void Replace(IServiceCollection services)
    {
        // Redis (HybridCache L2) → in-process distributed cache, still serialized like the real one.
        services.RemoveAll<IDistributedCache>();
        services.AddDistributedMemoryCache();

        // Entra client credentials (service-to-service) → test token with the Geo.Read role.
        services.RemoveAll<IAccessTokenProvider>();
        services.AddSingleton<IAccessTokenProvider, TestTokenProvider>();
    }

    private sealed class TestTokenProvider : IAccessTokenProvider
    {
        public Task<string> GetTokenAsync(CancellationToken cancellationToken) =>
            Task.FromResult(TestAuthHandler.TokenFor("Geo.Read"));
    }
}
