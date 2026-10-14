using DiscoverCostaRica.Testing.Containers;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscoverCostaRica.IntegrationTests.Caching;

/// <summary>The hybrid cache shares entries across instances through Redis (L2), as replicas in Container Apps would.</summary>
public sealed class CatalogHybridCacheTests(RedisFixture redis)
{
    [Fact]
    public async Task EntryWrittenByOneInstance_IsReadByAnotherThroughRedis()
    {
        var ct = TestContext.Current.CancellationToken;
        var key = $"test:{Guid.NewGuid():N}";
        using var writer = BuildHost();
        using var reader = BuildHost();

        await writer.Services.GetRequiredService<HybridCache>().SetAsync(key, "Arenal", cancellationToken: ct);

        var loads = 0;
        var value = await reader.Services.GetRequiredService<HybridCache>().GetOrCreateAsync(
            key,
            _ => { loads++; return ValueTask.FromResult("from the database"); },
            cancellationToken: ct);

        Assert.Equal("Arenal", value);
        Assert.Equal(0, loads);
    }

    private IHost BuildHost()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration["ConnectionStrings:redis"] = redis.ConnectionString;
        builder.AddRedisClient("redis");
        builder.AddCatalogHybridCache();
        return builder.Build();
    }
}
