using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Microsoft.Extensions.Hosting;

public static class CatalogHybridCacheExtensions
{
    /// <summary>Default lifetime of a cached catalog entry (spec 006 FR-007).</summary>
    public static readonly TimeSpan DefaultExpiration = TimeSpan.FromHours(24);

    /// <summary>
    /// Registers <see cref="HybridCache"/>: in-process L1 plus Redis L2, stampede protection and a TTL on
    /// every entry. Reuses the Redis connection that ServiceDefaults already registers.
    /// </summary>
    public static IHostApplicationBuilder AddCatalogHybridCache(this IHostApplicationBuilder builder)
    {
        builder.Services.AddStackExchangeRedisCache(_ => { });
        builder.Services.AddOptions<RedisCacheOptions>()
            .Configure<IServiceProvider>((options, provider) =>
                options.ConnectionMultiplexerFactory = () => Task.FromResult(provider.GetRequiredService<IConnectionMultiplexer>()));

        builder.Services.AddHybridCache(options =>
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = DefaultExpiration,
                LocalCacheExpiration = TimeSpan.FromMinutes(5),
            });

        return builder;
    }
}
