using Testcontainers.Redis;
using Xunit;

namespace DiscoverCostaRica.Testing.Containers;

/// <summary>One Redis container per test assembly.</summary>
public sealed class RedisFixture : IAsyncLifetime
{
    // Docker Hub's official image through Google's mirror, which avoids anonymous pull rate limits.
    private const string Image = "mirror.gcr.io/library/redis:7.4-alpine";

    private readonly RedisContainer _container = new RedisBuilder(Image).Build();

    public string ConnectionString => _container.GetConnectionString();

    public async ValueTask InitializeAsync() => await _container.StartAsync();

    public async ValueTask DisposeAsync() => await _container.DisposeAsync();
}
