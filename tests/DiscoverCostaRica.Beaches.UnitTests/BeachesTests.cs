using DiscoverCostaRica.Beaches.Application.Ports;
using DiscoverCostaRica.Beaches.Application.UseCases.GetBeaches;
using DiscoverCostaRica.Beaches.Domain;
using DiscoverCostaRica.BuildingBlocks.Application.Results;

namespace DiscoverCostaRica.Beaches.UnitTests;

public sealed class BeachesTests
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public void Beach_EnforcesItsInvariants()
    {
        Assert.Equal("Tamarindo", Beach.Create(1, "Tamarindo", "Surf town.").Name);
        Assert.Throws<ArgumentOutOfRangeException>(() => Beach.Create(0, "Tamarindo", "Surf town."));
        Assert.ThrowsAny<ArgumentException>(() => Beach.Create(1, " ", "Surf town."));
        Assert.ThrowsAny<ArgumentException>(() => Beach.Create(1, "Tamarindo", ""));
        Assert.Throws<ArgumentOutOfRangeException>(() => Beach.Create(1, new string('a', Beach.MaxNameLength + 1), "Surf town."));
        Assert.Throws<ArgumentOutOfRangeException>(() => Beach.Create(1, "Tamarindo", new string('a', Beach.MaxDescriptionLength + 1)));
    }

    [Fact]
    public async Task GetBeaches_MapsEveryBeach()
    {
        var repository = new FakeBeaches(Beach.Create(1, "Manuel Antonio", "National park."), Beach.Create(2, "Tamarindo", "Surf town."));

        var result = await new GetBeachesInteractor(repository).ExecuteAsync(GetBeachesRequest.Instance, Ct);

        Assert.Equal([new BeachResponse(1, "Manuel Antonio", "National park."), new BeachResponse(2, "Tamarindo", "Surf town.")], result.Value);
    }

    [Fact]
    public async Task GetBeaches_WhenEmpty_IsNotFound()
    {
        var result = await new GetBeachesInteractor(new FakeBeaches()).ExecuteAsync(GetBeachesRequest.Instance, Ct);

        Assert.Equal(Error.NotFound("No beaches found. Please check later for updates."), result.Error);
    }

    private sealed class FakeBeaches(params Beach[] beaches) : IBeachReadRepository
    {
        public Task<IReadOnlyList<Beach>> GetAllAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Beach>>(beaches);
    }
}
