using DiscoverCostaRica.BuildingBlocks.Application.Results;
using DiscoverCostaRica.Geo.Application.UseCases;
using DiscoverCostaRica.Geo.Application.UseCases.GetCantonById;
using DiscoverCostaRica.Geo.Application.UseCases.GetCantonsByProvince;
using DiscoverCostaRica.Geo.Application.UseCases.GetDistrictById;
using DiscoverCostaRica.Geo.Application.UseCases.GetDistrictsByCanton;
using DiscoverCostaRica.Geo.Application.UseCases.GetProvinceById;
using DiscoverCostaRica.Geo.Application.UseCases.GetProvinces;
using DiscoverCostaRica.Geo.UnitTests.Fakes;

namespace DiscoverCostaRica.Geo.UnitTests.UseCases;

public sealed class LookupInteractorTests
{
    private readonly FakeGeoReadRepository _repository = new();
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task GetProvinces_ReturnsEveryProvince()
    {
        var result = await new GetProvincesInteractor(_repository).ExecuteAsync(GetProvincesRequest.Instance, Ct);

        Assert.Equal([new DivisionResponse(1, "San José"), new DivisionResponse(2, "Alajuela")], result.Value);
    }

    [Fact]
    public async Task GetProvinces_WhenEmpty_IsNotFound()
    {
        _repository.Provinces.Clear();

        var result = await new GetProvincesInteractor(_repository).ExecuteAsync(GetProvincesRequest.Instance, Ct);

        Assert.Equal(Error.NotFound("No provinces found. Please check later for updates."), result.Error);
    }

    [Theory]
    [InlineData(2, true)]
    [InlineData(3, false)]
    [InlineData(9, false)]
    public async Task GetProvinceById_FindsKnownCodesOnly(int provinceId, bool found)
    {
        var result = await new GetProvinceByIdInteractor(_repository).ExecuteAsync(new GetProvinceByIdRequest(provinceId), Ct);

        Assert.Equal(found, result.IsSuccess);
        if (!found) Assert.Equal($"No province was found for {provinceId}", result.Error!.Message);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 1)]
    [InlineData(5, 0)]
    [InlineData(9, 0)]
    public async Task GetCantonsByProvince_ReturnsOnlyThatProvince(int provinceId, int expected)
    {
        var result = await new GetCantonsByProvinceInteractor(_repository).ExecuteAsync(new GetCantonsByProvinceRequest(provinceId), Ct);

        Assert.Equal(expected, result.Value?.Count ?? 0);
        if (expected == 0) Assert.Equal(GetCantonsByProvinceInteractor.NotFoundMessage, result.Error!.Message);
    }

    [Theory]
    [InlineData(2, 1, "Alajuela")]
    [InlineData(1, 1, "San José")]
    [InlineData(1, 9, null)]
    [InlineData(0, 1, null)]
    public async Task GetCantonById_UsesProvinceAndCanton(int provinceId, int cantonId, string? name)
    {
        var result = await new GetCantonByIdInteractor(_repository).ExecuteAsync(new GetCantonByIdRequest(provinceId, cantonId), Ct);

        Assert.Equal(name, result.Value?.Name);
        if (name is null) Assert.Equal($"No canton was found for {cantonId}", result.Error!.Message);
    }

    [Fact]
    public async Task GetDistrictsByCanton_WithProvince_DoesNotLeakOtherProvinces()
    {
        var result = await new GetDistrictsByCantonInteractor(_repository).ExecuteAsync(new GetDistrictsByCantonRequest(1, 1), Ct);

        Assert.Equal(["Carmen", "Merced"], result.Value!.Select(d => d.Name));
    }

    [Fact]
    public async Task GetDistrictsByCanton_WithoutProvince_KeepsTheDeprecatedBehaviour()
    {
        var result = await new GetDistrictsByCantonInteractor(_repository).ExecuteAsync(new GetDistrictsByCantonRequest(null, 1), Ct);

        Assert.Equal(["Carmen", "Merced", "Alajuela"], result.Value!.Select(d => d.Name));
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(9, 1)]
    [InlineData(null, 9)]
    public async Task GetDistrictsByCanton_WithNoMatch_IsNotFound(int? provinceId, int cantonId)
    {
        var result = await new GetDistrictsByCantonInteractor(_repository).ExecuteAsync(new GetDistrictsByCantonRequest(provinceId, cantonId), Ct);

        Assert.Equal(Error.NotFound(GetDistrictsByCantonInteractor.NotFoundMessage), result.Error);
    }

    [Theory]
    [InlineData(2, 1, 1, "Alajuela")]
    [InlineData(1, 1, 2, "Merced")]
    [InlineData(null, 1, 1, "Carmen")]
    [InlineData(1, 1, 9, null)]
    [InlineData(9, 1, 1, null)]
    [InlineData(null, 7, 7, null)]
    public async Task GetDistrictById_ResolvesScopedAndDeprecatedLookups(int? provinceId, int cantonId, int districtId, string? name)
    {
        var result = await new GetDistrictByIdInteractor(_repository).ExecuteAsync(new GetDistrictByIdRequest(provinceId, cantonId, districtId), Ct);

        Assert.Equal(name, result.Value?.Name);
        if (name is null) Assert.Equal($"No district was found for {districtId}", result.Error!.Message);
    }
}
