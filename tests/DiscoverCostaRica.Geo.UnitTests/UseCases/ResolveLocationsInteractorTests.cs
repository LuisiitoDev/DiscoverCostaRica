using DiscoverCostaRica.BuildingBlocks.Application.Results;
using DiscoverCostaRica.Geo.Application.UseCases;
using DiscoverCostaRica.Geo.Application.UseCases.ResolveLocations;
using DiscoverCostaRica.Geo.UnitTests.Fakes;

namespace DiscoverCostaRica.Geo.UnitTests.UseCases;

public sealed class ResolveLocationsInteractorTests
{
    private readonly FakeGeoReadRepository _repository = new();
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task ResolvesEachItemInOrder_WithThreeQueriesWhateverTheBatchSize()
    {
        var items = new List<LocationQuery>
        {
            new(2, 1, 1),
            new(1, 2, null),
            new(1, 1, 99),
            new(9, 1, 1),
        };
        items.AddRange(Enumerable.Repeat(new LocationQuery(1, 1, 1), 100));

        var result = await new ResolveLocationsInteractor(_repository).ExecuteAsync(new ResolveLocationsRequest(items), Ct);

        var resolved = result.Value!.Items;
        Assert.Equal(104, resolved.Count);
        Assert.Equal(new ResolvedLocation(new(2, "Alajuela"), new(1, "Alajuela"), new(1, "Alajuela")), resolved[0]);
        Assert.Equal(new ResolvedLocation(new(1, "San José"), new(2, "Escazú"), null), resolved[1]);
        Assert.Equal(new ResolvedLocation(new(1, "San José"), new(1, "San José"), null), resolved[2]);
        Assert.Equal(new ResolvedLocation(null, null, null), resolved[3]);
        Assert.All(resolved.Skip(4), r => Assert.Equal(new DivisionResponse(1, "Carmen"), r.District));
        Assert.Equal(3, _repository.BatchQueries);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(201)]
    public async Task RejectsBatchesOutsideTheLimit(int size)
    {
        var items = Enumerable.Repeat(new LocationQuery(1, 1, null), size).ToList();

        var result = await new ResolveLocationsInteractor(_repository).ExecuteAsync(new ResolveLocationsRequest(items), Ct);

        Assert.Equal(Error.Validation("items must contain between 1 and 200 entries"), result.Error);
        Assert.Equal(0, _repository.BatchQueries);
    }

    [Fact]
    public async Task RejectsAMissingList()
    {
        var result = await new ResolveLocationsInteractor(_repository).ExecuteAsync(new ResolveLocationsRequest(null), Ct);

        Assert.Equal(ErrorType.Validation, result.Error!.Type);
    }
}
