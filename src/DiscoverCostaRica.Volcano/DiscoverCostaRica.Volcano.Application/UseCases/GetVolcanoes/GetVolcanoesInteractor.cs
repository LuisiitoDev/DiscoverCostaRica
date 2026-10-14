using DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection;
using DiscoverCostaRica.BuildingBlocks.Application.Results;
using DiscoverCostaRica.BuildingBlocks.Application.UseCases;
using DiscoverCostaRica.Volcano.Application.Ports;

namespace DiscoverCostaRica.Volcano.Application.UseCases.GetVolcanoes;

[TransientService]
public sealed class GetVolcanoesInteractor(IVolcanoReadRepository repository, IGeoLocationGateway geo)
    : IUseCase<GetVolcanoesRequest, IReadOnlyList<VolcanoResponse>>
{
    public const string NotFoundMessage = "Volcanos were not found";

    public async Task<Result<IReadOnlyList<VolcanoResponse>>> ExecuteAsync(GetVolcanoesRequest request, CancellationToken cancellationToken)
    {
        var volcanoes = await repository.GetAllAsync(cancellationToken);
        if (volcanoes.Count == 0)
            return Error.NotFound(NotFoundMessage);

        return Result<IReadOnlyList<VolcanoResponse>>.Success(await geo.WithLocationsAsync(volcanoes, cancellationToken));
    }
}
