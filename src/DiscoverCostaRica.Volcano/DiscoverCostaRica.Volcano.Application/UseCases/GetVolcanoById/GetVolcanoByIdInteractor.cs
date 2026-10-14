using DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection;
using DiscoverCostaRica.BuildingBlocks.Application.Results;
using DiscoverCostaRica.BuildingBlocks.Application.UseCases;
using DiscoverCostaRica.Volcano.Application.Ports;

namespace DiscoverCostaRica.Volcano.Application.UseCases.GetVolcanoById;

[TransientService]
public sealed class GetVolcanoByIdInteractor(IVolcanoReadRepository repository, IGeoLocationGateway geo)
    : IUseCase<GetVolcanoByIdRequest, VolcanoResponse>
{
    public const string NotFoundMessage = "Volcano was not found";

    public async Task<Result<VolcanoResponse>> ExecuteAsync(GetVolcanoByIdRequest request, CancellationToken cancellationToken)
    {
        var volcano = await repository.FindAsync(request.Id, cancellationToken);
        if (volcano is null)
            return Error.NotFound(NotFoundMessage);

        // Same shape as the list, location included (VOL-D5).
        return (await geo.WithLocationsAsync([volcano], cancellationToken))[0];
    }
}
