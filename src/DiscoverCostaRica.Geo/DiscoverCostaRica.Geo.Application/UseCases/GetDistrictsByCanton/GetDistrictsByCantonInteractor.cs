using DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection;
using DiscoverCostaRica.BuildingBlocks.Application.Results;
using DiscoverCostaRica.BuildingBlocks.Application.UseCases;
using DiscoverCostaRica.Geo.Application.Ports;
using DiscoverCostaRica.Geo.Domain;
using DiscoverCostaRica.Geo.Domain.ValueObjects;

namespace DiscoverCostaRica.Geo.Application.UseCases.GetDistrictsByCanton;

[TransientService]
public sealed class GetDistrictsByCantonInteractor(IGeoReadRepository repository)
    : IUseCase<GetDistrictsByCantonRequest, IReadOnlyList<DivisionResponse>>
{
    public const string NotFoundMessage = "No districts found. Please check later for updates.";

    public async Task<Result<IReadOnlyList<DivisionResponse>>> ExecuteAsync(GetDistrictsByCantonRequest request, CancellationToken cancellationToken)
    {
        IReadOnlyList<District> districts;

        if (request.ProvinceId is not { } provinceId)
        {
            districts = await repository.GetDistrictsInAnyProvinceAsync(request.CantonId, cancellationToken);
        }
        else
        {
            if (!CantonCode.TryCreate(provinceId, request.CantonId, out var canton))
                return Error.NotFound(NotFoundMessage);

            districts = await repository.GetDistrictsAsync(canton, cancellationToken);
        }

        return districts.Count == 0
            ? Error.NotFound(NotFoundMessage)
            : Result<IReadOnlyList<DivisionResponse>>.Success([.. districts.Select(DivisionResponse.From)]);
    }
}
