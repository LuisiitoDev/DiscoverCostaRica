using System.Collections.Concurrent;
using System.Text.Json;
using DiscoverCostaRica.BuildingBlocks.Infrastructure.Authentication;
using DiscoverCostaRica.Shared.Interfaces;
using DiscoverCostaRica.Shared.logging;
using DiscoverCostaRica.Testing.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DiscoverCostaRica.CharacterizationTests.Harness;

/// <summary>
/// The only place that knows how the services reach remote infrastructure.
/// When a service migrates (spec 006) this file changes; the tests do not.
/// </summary>
internal static class InfrastructureSeams
{
    public static void Replace(IServiceCollection services)
    {
        // Redis → in-memory, with the same JSON round trip the real cache does.
        services.RemoveAll<ICacheService>();
        services.AddSingleton<ICacheService, InMemoryJsonCache>();

        // MongoDB error sink → removed.
        var mongoLogger = services.Where(d => d.ImplementationType == typeof(DiscoverCostaRicaLoggerProvider)).ToList();
        foreach (var descriptor in mongoLogger)
            services.Remove(descriptor);

        // Entra client credentials (service-to-service) → test token with the Geo.Read role.
        services.RemoveAll<IAccessTokenProvider>();
        services.AddSingleton<IAccessTokenProvider, TestTokenProvider>();
    }

    private sealed class InMemoryJsonCache : ICacheService
    {
        private readonly ConcurrentDictionary<string, string> _entries = new();

        public Task<TSource?> Get<TSource>(string key, CancellationToken cancellationToken) =>
            Task.FromResult(_entries.TryGetValue(key, out var json) ? JsonSerializer.Deserialize<TSource>(json) : default);

        public Task Set<TSource>(string key, TSource source, CancellationToken cancellationToken)
        {
            _entries[key] = JsonSerializer.Serialize(source);
            return Task.CompletedTask;
        }
    }

    private sealed class TestTokenProvider : IAccessTokenProvider
    {
        public Task<string> GetTokenAsync(CancellationToken cancellationToken) =>
            Task.FromResult(TestAuthHandler.TokenFor("Geo.Read"));
    }
}

