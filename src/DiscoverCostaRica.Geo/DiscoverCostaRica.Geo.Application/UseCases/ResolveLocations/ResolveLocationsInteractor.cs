using DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection;
using DiscoverCostaRica.BuildingBlocks.Application.Results;
using DiscoverCostaRica.BuildingBlocks.Application.UseCases;
using DiscoverCostaRica.Geo.Application.Ports;
using DiscoverCostaRica.Geo.Domain.ValueObjects;

namespace DiscoverCostaRica.Geo.Application.UseCases.ResolveLocations;

/// <summary>
/// Resolves many locations at once so consumers make one call instead of three per item
/// (VOL-D4). Cost is three set-based queries whatever the batch size.
/// </summary>
[TransientService]
public sealed class ResolveLocationsInteractor(IGeoReadRepository repository)
    : IUseCase<ResolveLocationsRequest, ResolveLocationsResponse>
{
    public const int MaxItems = 200;
    public static readonly string InvalidSizeMessage = $"items must contain between 1 and {MaxItems} entries";

    public async Task<Result<ResolveLocationsResponse>> ExecuteAsync(ResolveLocationsRequest request, CancellationToken cancellationToken)
    {
        if (request.Items is not { Count: > 0 and <= MaxItems } items)
            return Error.Validation(InvalidSizeMessage);

        var codes = items.Select(Parse).ToList();

        var provinces = (await repository.GetProvincesByCodeAsync(Distinct(codes, c => c.Province), cancellationToken))
            .ToDictionary(p => p.Code, DivisionResponse.From);
        var cantons = (await repository.GetCantonsByCodeAsync(Distinct(codes, c => c.Canton), cancellationToken))
            .ToDictionary(c => c.Code, DivisionResponse.From);
        var districts = (await repository.GetDistrictsByCodeAsync(Distinct(codes, c => c.District), cancellationToken))
            .ToDictionary(d => d.Code, DivisionResponse.From);

        var resolved = codes.Select(c => new ResolvedLocation(
            Lookup(provinces, c.Province),
            Lookup(cantons, c.Canton),
            Lookup(districts, c.District)));

        return new ResolveLocationsResponse([.. resolved]);
    }

    private static Codes Parse(LocationQuery query)
    {
        ProvinceCode.TryCreate(query.ProvinceId, out var province);
        CantonCode.TryCreate(query.ProvinceId, query.CantonId, out var canton);
        DistrictCode? district = null;
        if (query.DistrictId is { } districtId)
            DistrictCode.TryCreate(query.ProvinceId, query.CantonId, districtId, out district);

        return new Codes(province, canton, district);
    }

    private static List<T> Distinct<T>(IEnumerable<Codes> codes, Func<Codes, T?> select) where T : class =>
        [.. codes.Select(select).OfType<T>().Distinct()];

    private static DivisionResponse? Lookup<T>(Dictionary<T, DivisionResponse> names, T? code) where T : class =>
        code is not null && names.TryGetValue(code, out var name) ? name : null;

    private sealed record Codes(ProvinceCode? Province, CantonCode? Canton, DistrictCode? District);
}
