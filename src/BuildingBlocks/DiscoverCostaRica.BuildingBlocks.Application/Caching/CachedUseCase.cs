using DiscoverCostaRica.BuildingBlocks.Application.Results;
using DiscoverCostaRica.BuildingBlocks.Application.UseCases;
using Microsoft.Extensions.Caching.Hybrid;

namespace DiscoverCostaRica.BuildingBlocks.Application.Caching;

/// <summary>
/// Cache-aside decorator for a read use case. Only successes are cached, and concurrent misses on
/// the same key run the inner use case once (HybridCache stampede protection).
/// Lifetime defaults to the TTL configured for the application (24 h, spec 006 FR-007).
/// </summary>
public abstract class CachedUseCase<TRequest, TResponse>(IUseCase<TRequest, TResponse> inner, HybridCache cache)
    : IUseCase<TRequest, TResponse>
{
    /// <summary>Cache key for the request. Prefix it with the owning service, e.g. <c>geo:provinces</c>.</summary>
    protected abstract string KeyFor(TRequest request);

    /// <summary>Per-use-case lifetime; null keeps the application default.</summary>
    protected virtual HybridCacheEntryOptions? EntryOptions => null;

    /// <summary>Tags for bulk invalidation, e.g. the service name.</summary>
    protected virtual IEnumerable<string>? Tags => null;

    public async Task<Result<TResponse>> ExecuteAsync(TRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return await cache.GetOrCreateAsync(
                KeyFor(request),
                (inner, request),
                static async (state, token) =>
                {
                    var result = await state.inner.ExecuteAsync(state.request, token);

                    // Throwing is the only way to stop HybridCache from storing the factory's outcome.
                    return result.IsSuccess ? result.Value : throw new FailedResultException(result.Error);
                },
                EntryOptions,
                Tags,
                cancellationToken);
        }
        catch (FailedResultException failed)
        {
            return failed.Error;
        }
    }

    private sealed class FailedResultException(Error error) : Exception(error.Message)
    {
        public Error Error { get; } = error;
    }
}
