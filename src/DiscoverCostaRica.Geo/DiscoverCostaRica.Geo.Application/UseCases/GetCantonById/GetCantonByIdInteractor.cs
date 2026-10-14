using DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection;
using DiscoverCostaRica.BuildingBlocks.Application.Results;
using DiscoverCostaRica.BuildingBlocks.Application.UseCases;
using DiscoverCostaRica.Geo.Application.Ports;
using DiscoverCostaRica.Geo.Domain.ValueObjects;

namespace DiscoverCostaRica.Geo.Application.UseCases.GetCantonById;

[TransientService]
public sealed class GetCantonByIdInteractor(IGeoReadRepository repository)
    : IUseCase<GetCantonByIdRequest, DivisionResponse>
{
    public static string NotFoundMessage(int cantonId) => $"No canton was found for {cantonId}";

    public async Task<Result<DivisionResponse>> ExecuteAsync(GetCantonByIdRequest request, CancellationToken cancellationToken)
    {
        if (!CantonCode.TryCreate(request.ProvinceId, request.CantonId, out var code))
            return Error.NotFound(NotFoundMessage(request.CantonId));

        var canton = await repository.FindCantonAsync(code, cancellationToken);

        return canton is null ? Error.NotFound(NotFoundMessage(request.CantonId)) : DivisionResponse.From(canton);
    }
}
