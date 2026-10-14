using DiscoverCostaRica.Beaches.Application.Ports;
using DiscoverCostaRica.BuildingBlocks.Application.Caching;
using DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection;
using DiscoverCostaRica.BuildingBlocks.Application.Results;
using DiscoverCostaRica.BuildingBlocks.Application.UseCases;
using Microsoft.Extensions.Caching.Hybrid;

namespace DiscoverCostaRica.Beaches.Application.UseCases.GetBeaches;

public sealed record GetBeachesRequest
{
    public static GetBeachesRequest Instance { get; } = new();
}

public sealed record BeachResponse(int Id, string Name, string Description);

[TransientService]
public sealed class GetBeachesInteractor(IBeachReadRepository repository)
    : IUseCase<GetBeachesRequest, IReadOnlyList<BeachResponse>>
{
    public const string NotFoundMessage = "No beaches found. Please check later for updates.";

    public async Task<Result<IReadOnlyList<BeachResponse>>> ExecuteAsync(GetBeachesRequest request, CancellationToken cancellationToken)
    {
        var beaches = await repository.GetAllAsync(cancellationToken);

        return beaches.Count == 0
            ? Error.NotFound(NotFoundMessage)
            : Result<IReadOnlyList<BeachResponse>>.Success([.. beaches.Select(b => new BeachResponse(b.Id, b.Name, b.Description))]);
    }
}

/// <summary>Cached for the default TTL (BCH-D2).</summary>
[DecoratorService]
public sealed class CachedGetBeaches(IUseCase<GetBeachesRequest, IReadOnlyList<BeachResponse>> inner, HybridCache cache)
    : CachedUseCase<GetBeachesRequest, IReadOnlyList<BeachResponse>>(inner, cache)
{
    public const string Key = "beach:beaches";

    protected override string KeyFor(GetBeachesRequest request) => Key;

    protected override IEnumerable<string> Tags => ["beach"];
}
