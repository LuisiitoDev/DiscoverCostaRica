using DiscoverCostaRica.BuildingBlocks.Application.Results;
using DiscoverCostaRica.Volcano.Application.UseCases;
using DiscoverCostaRica.Volcano.Application.UseCases.GetVolcanoById;
using DiscoverCostaRica.Volcano.Application.UseCases.GetVolcanoes;
using DiscoverCostaRica.Volcano.Application.UseCases.GetVolcanoesByProvince;
using DiscoverCostaRica.Volcano.Domain;

namespace DiscoverCostaRica.Volcano.UnitTests;

public sealed class InteractorTests
{
    private readonly FakeVolcanoReadRepository _repository = new();
    private readonly FakeGeoLocationGateway _geo = new();
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task GetVolcanoes_ResolvesDistinctCodesInOneCall()
    {
        var result = await new GetVolcanoesInteractor(_repository, _geo).ExecuteAsync(GetVolcanoesRequest.Instance, Ct);

        var calls = Assert.Single(_geo.Calls);
        Assert.Equal([LocationCode.Create(2, 10, 7), LocationCode.Create(2, 8)], calls);
        Assert.Equal(3, result.Value!.Count);
        Assert.Equal("La Fortuna", result.Value[0].Location!.District!.Name);
        Assert.Null(result.Value[1].Location);
    }

    [Fact]
    public async Task GetVolcanoes_WhenEmpty_IsNotFoundWithoutCallingGeo()
    {
        _repository.Volcanoes.Clear();

        var result = await new GetVolcanoesInteractor(_repository, _geo).ExecuteAsync(GetVolcanoesRequest.Instance, Ct);

        Assert.Equal(Error.NotFound("Volcanos were not found"), result.Error);
        Assert.Empty(_geo.Calls);
    }

    [Fact]
    public async Task GetVolcanoById_IncludesTheLocation()
    {
        var result = await new GetVolcanoByIdInteractor(_repository, _geo).ExecuteAsync(new GetVolcanoByIdRequest(1), Ct);

        Assert.Equal(
            new VolcanoResponse(1, "Arenal", "Stratovolcano.", new(new(2, "Alajuela"), new(10, "San Carlos"), new(7, "La Fortuna"))),
            result.Value);
    }

    [Fact]
    public async Task GetVolcanoById_Unknown_IsNotFound()
    {
        var result = await new GetVolcanoByIdInteractor(_repository, _geo).ExecuteAsync(new GetVolcanoByIdRequest(42), Ct);

        Assert.Equal(Error.NotFound("Volcano was not found"), result.Error);
    }

    [Theory]
    [InlineData(2, 3)]
    [InlineData(1, 0)]
    public async Task GetVolcanoesByProvince_FiltersByProvince(int provinceId, int expected)
    {
        var result = await new GetVolcanoesByProvinceInteractor(_repository, _geo).ExecuteAsync(new GetVolcanoesByProvinceRequest(provinceId), Ct);

        Assert.Equal(expected, result.Value?.Count ?? 0);
        if (expected == 0) Assert.Equal("Volcanos were not found", result.Error!.Message);
    }
}
