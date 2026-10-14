using DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection;
using DiscoverCostaRica.BuildingBlocks.Application.Results;
using DiscoverCostaRica.BuildingBlocks.Application.UseCases;
using DiscoverCostaRica.Geo.Application.Ports;
using DiscoverCostaRica.Geo.Domain.ValueObjects;

namespace DiscoverCostaRica.Geo.Application.UseCases.GetProvinceById;

[TransientService]
public sealed class GetProvinceByIdInteractor(IGeoReadRepository repository)
    : IUseCase<GetProvinceByIdRequest, DivisionResponse>
{
    public static string NotFoundMessage(int provinceId) => $"No province was found for {provinceId}";

    public async Task<Result<DivisionResponse>> ExecuteAsync(GetProvinceByIdRequest request, CancellationToken cancellationToken)
    {
        // A code outside 1..7 cannot name a province: same answer as an unknown one.
        if (!ProvinceCode.TryCreate(request.ProvinceId, out var code))
            return Error.NotFound(NotFoundMessage(request.ProvinceId));

        var province = await repository.FindProvinceAsync(code, cancellationToken);

        return province is null ? Error.NotFound(NotFoundMessage(request.ProvinceId)) : DivisionResponse.From(province);
    }
}
