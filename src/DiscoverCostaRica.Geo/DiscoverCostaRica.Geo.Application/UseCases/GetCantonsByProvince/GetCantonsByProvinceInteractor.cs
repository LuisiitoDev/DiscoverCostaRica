using DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection;
using DiscoverCostaRica.BuildingBlocks.Application.Results;
using DiscoverCostaRica.BuildingBlocks.Application.UseCases;
using DiscoverCostaRica.Geo.Application.Ports;
using DiscoverCostaRica.Geo.Domain.ValueObjects;

namespace DiscoverCostaRica.Geo.Application.UseCases.GetCantonsByProvince;

[TransientService]
public sealed class GetCantonsByProvinceInteractor(IGeoReadRepository repository)
    : IUseCase<GetCantonsByProvinceRequest, IReadOnlyList<DivisionResponse>>
{
    public const string NotFoundMessage = "No cantons found. Please check later for updates.";

    public async Task<Result<IReadOnlyList<DivisionResponse>>> ExecuteAsync(GetCantonsByProvinceRequest request, CancellationToken cancellationToken)
    {
        if (!ProvinceCode.TryCreate(request.ProvinceId, out var province))
            return Error.NotFound(NotFoundMessage);

        var cantons = await repository.GetCantonsAsync(province, cancellationToken);

        return cantons.Count == 0
            ? Error.NotFound(NotFoundMessage)
            : Result<IReadOnlyList<DivisionResponse>>.Success([.. cantons.Select(DivisionResponse.From)]);
    }
}
