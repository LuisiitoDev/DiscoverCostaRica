using DiscoverCostaRica.BuildingBlocks.Application.Caching;
using DiscoverCostaRica.BuildingBlocks.Application.Results;
using DiscoverCostaRica.BuildingBlocks.Application.UseCases;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;

namespace DiscoverCostaRica.BuildingBlocks.UnitTests.Application;

public sealed class CachedUseCaseTests
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Success_IsCached()
    {
        var inner = new CountingUseCase(_ => "Arenal");
        var cached = new CachedEcho(inner, NewCache());

        var first = await cached.ExecuteAsync("volcano:1", Ct);
        var second = await cached.ExecuteAsync("volcano:1", Ct);

        Assert.Equal("Arenal", first.Value);
        Assert.Equal("Arenal", second.Value);
        Assert.Equal(1, inner.Calls);
    }

    [Fact]
    public async Task Failure_IsReturnedButNotCached()
    {
        var inner = new CountingUseCase(_ => Error.NotFound("Volcano was not found"));
        var cached = new CachedEcho(inner, NewCache());

        var first = await cached.ExecuteAsync("volcano:99", Ct);
        var second = await cached.ExecuteAsync("volcano:99", Ct);

        Assert.Equal(Error.NotFound("Volcano was not found"), first.Error);
        Assert.Equal(Error.NotFound("Volcano was not found"), second.Error);
        Assert.Equal(2, inner.Calls);
    }

    [Fact]
    public async Task ConcurrentMisses_RunTheInnerUseCaseOnce()
    {
        var gate = new TaskCompletionSource();
        var inner = new CountingUseCase(_ => "Poás", gate.Task);
        var cached = new CachedEcho(inner, NewCache());

        var calls = Enumerable.Range(0, 10).Select(_ => cached.ExecuteAsync("volcano:2", Ct)).ToList();
        gate.SetResult();
        var results = await Task.WhenAll(calls);

        Assert.All(results, r => Assert.Equal("Poás", r.Value));
        Assert.Equal(1, inner.Calls);
    }

    private static HybridCache NewCache() =>
        new ServiceCollection().AddHybridCache().Services.BuildServiceProvider().GetRequiredService<HybridCache>();

    private sealed class CachedEcho(IUseCase<string, string> inner, HybridCache cache) : CachedUseCase<string, string>(inner, cache)
    {
        protected override string KeyFor(string request) => request;
    }

    private sealed class CountingUseCase(Func<string, Result<string>> answer, Task? gate = null) : IUseCase<string, string>
    {
        private int _calls;

        public int Calls => _calls;

        public async Task<Result<string>> ExecuteAsync(string request, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _calls);
            if (gate is not null) await gate;
            return answer(request);
        }
    }
}
