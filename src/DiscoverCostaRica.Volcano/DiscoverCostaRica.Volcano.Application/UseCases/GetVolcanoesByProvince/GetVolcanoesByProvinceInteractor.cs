using DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection;
using DiscoverCostaRica.BuildingBlocks.Application.Results;
using DiscoverCostaRica.BuildingBlocks.Application.UseCases;
using DiscoverCostaRica.Volcano.Application.Ports;

namespace DiscoverCostaRica.Volcano.Application.UseCases.GetVolcanoesByProvince;

[TransientService]
public sealed class GetVolcanoesByProvinceInteractor(IVolcanoReadRepository repository, IGeoLocationGateway geo)
    : IUseCase<GetVolcanoesByProvinceRequest, IReadOnlyList<VolcanoResponse>>
{
    public const string NotFoundMessage = "Volcanos were not found";

    public async Task<Result<IReadOnlyList<VolcanoResponse>>> ExecuteAsync(GetVolcanoesByProvinceRequest request, CancellationToken cancellationToken)
    {
        var volcanoes = await repository.GetByProvinceAsync(request.ProvinceId, cancellationToken);
        if (volcanoes.Count == 0)
            return Error.NotFound(NotFoundMessage);

        return Result<IReadOnlyList<VolcanoResponse>>.Success(await geo.WithLocationsAsync(volcanoes, cancellationToken));
    }
}
