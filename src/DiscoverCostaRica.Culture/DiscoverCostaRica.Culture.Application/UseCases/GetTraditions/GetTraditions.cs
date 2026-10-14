using DiscoverCostaRica.BuildingBlocks.Application.Caching;
using DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection;
using DiscoverCostaRica.BuildingBlocks.Application.Results;
using DiscoverCostaRica.BuildingBlocks.Application.UseCases;
using DiscoverCostaRica.Culture.Application.Ports;
using Microsoft.Extensions.Caching.Hybrid;

namespace DiscoverCostaRica.Culture.Application.UseCases.GetTraditions;

public sealed record GetTraditionsRequest
{
    public static GetTraditionsRequest Instance { get; } = new();
}

public sealed record TraditionResponse(int Id, string Name, string Description, string? ImageUrl);

[TransientService]
public sealed class GetTraditionsInteractor(ICultureReadRepository repository)
    : IUseCase<GetTraditionsRequest, IReadOnlyList<TraditionResponse>>
{
    public const string NotFoundMessage = "No traditions found. Please check later for updates.";

    public async Task<Result<IReadOnlyList<TraditionResponse>>> ExecuteAsync(GetTraditionsRequest request, CancellationToken cancellationToken)
    {
        var traditions = await repository.GetTraditionsAsync(cancellationToken);

        return traditions.Count == 0
            ? Error.NotFound(NotFoundMessage)
            : Result<IReadOnlyList<TraditionResponse>>.Success(
                [.. traditions.Select(t => new TraditionResponse(t.Id, t.Name, t.Description, t.ImageUrl))]);
    }
}

/// <summary>Cached for the default TTL (CUL-D2).</summary>
[DecoratorService]
public sealed class CachedGetTraditions(IUseCase<GetTraditionsRequest, IReadOnlyList<TraditionResponse>> inner, HybridCache cache)
    : CachedUseCase<GetTraditionsRequest, IReadOnlyList<TraditionResponse>>(inner, cache)
{
    public const string Key = "culture:traditions";

    protected override string KeyFor(GetTraditionsRequest request) => Key;

    protected override IEnumerable<string> Tags => ["culture"];
}
