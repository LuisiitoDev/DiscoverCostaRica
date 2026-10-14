using DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection;
using DiscoverCostaRica.BuildingBlocks.Application.Results;
using DiscoverCostaRica.BuildingBlocks.Application.UseCases;
using DiscoverCostaRica.Geo.Application.Ports;
using DiscoverCostaRica.Geo.Domain;
using DiscoverCostaRica.Geo.Domain.ValueObjects;

namespace DiscoverCostaRica.Geo.Application.UseCases.GetDistrictById;

[TransientService]
public sealed class GetDistrictByIdInteractor(IGeoReadRepository repository)
    : IUseCase<GetDistrictByIdRequest, DivisionResponse>
{
    public static string NotFoundMessage(int districtId) => $"No district was found for {districtId}";

    public async Task<Result<DivisionResponse>> ExecuteAsync(GetDistrictByIdRequest request, CancellationToken cancellationToken)
    {
        District? district;

        if (request.ProvinceId is not { } provinceId)
        {
            district = await repository.FindDistrictInAnyProvinceAsync(request.CantonId, request.DistrictId, cancellationToken);
        }
        else
        {
            if (!DistrictCode.TryCreate(provinceId, request.CantonId, request.DistrictId, out var code))
                return Error.NotFound(NotFoundMessage(request.DistrictId));

            district = await repository.FindDistrictAsync(code, cancellationToken);
        }

        return district is null ? Error.NotFound(NotFoundMessage(request.DistrictId)) : DivisionResponse.From(district);
    }
}
