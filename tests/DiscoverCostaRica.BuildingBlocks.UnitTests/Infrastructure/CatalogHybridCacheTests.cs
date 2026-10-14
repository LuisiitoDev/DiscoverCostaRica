using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DiscoverCostaRica.BuildingBlocks.UnitTests.Infrastructure;

public sealed class CatalogHybridCacheTests
{
    [Fact]
    public void EveryEntryGetsTheDefaultTtl()
    {
        var builder = Host.CreateApplicationBuilder();

        builder.AddCatalogHybridCache();
        using var host = builder.Build();

        var options = host.Services.GetRequiredService<IOptions<HybridCacheOptions>>().Value;
        Assert.Equal(TimeSpan.FromHours(24), options.DefaultEntryOptions?.Expiration);
        Assert.Equal(TimeSpan.FromMinutes(5), options.DefaultEntryOptions?.LocalCacheExpiration);
        Assert.NotNull(host.Services.GetService<HybridCache>());
    }
}
