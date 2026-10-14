using DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection;
using DiscoverCostaRica.BuildingBlocks.Application.Results;
using DiscoverCostaRica.BuildingBlocks.Application.UseCases;
using DiscoverCostaRica.Geo.Application.Ports;

namespace DiscoverCostaRica.Geo.Application.UseCases.GetProvinces;

[TransientService]
public sealed class GetProvincesInteractor(IGeoReadRepository repository)
    : IUseCase<GetProvincesRequest, IReadOnlyList<DivisionResponse>>
{
    public const string NotFoundMessage = "No provinces found. Please check later for updates.";

    public async Task<Result<IReadOnlyList<DivisionResponse>>> ExecuteAsync(GetProvincesRequest request, CancellationToken cancellationToken)
    {
        var provinces = await repository.GetProvincesAsync(cancellationToken);

        return provinces.Count == 0
            ? Error.NotFound(NotFoundMessage)
            : Result<IReadOnlyList<DivisionResponse>>.Success([.. provinces.Select(DivisionResponse.From)]);
    }
}
