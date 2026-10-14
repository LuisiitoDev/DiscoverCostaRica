using DiscoverCostaRica.BuildingBlocks.Application.Results;
using DiscoverCostaRica.Culture.Application.Ports;
using DiscoverCostaRica.Culture.Application.UseCases.GetDishes;
using DiscoverCostaRica.Culture.Application.UseCases.GetTraditions;
using DiscoverCostaRica.Culture.Domain;

namespace DiscoverCostaRica.Culture.UnitTests;

public sealed class CultureTests
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public void Tradition_EnforcesItsInvariants()
    {
        Assert.Null(Tradition.Create(1, "Día del Boyero", "Ox carts.").ImageUrl);
        Assert.Throws<ArgumentOutOfRangeException>(() => Tradition.Create(0, "Día del Boyero", "Ox carts."));
        Assert.ThrowsAny<ArgumentException>(() => Tradition.Create(1, "", "Ox carts."));
        Assert.ThrowsAny<ArgumentException>(() => Tradition.Create(1, "Día del Boyero", " "));
    }

    [Fact]
    public void Dish_EnforcesItsInvariants()
    {
        Assert.Equal("https://example.org/a.jpg", Dish.Create(1, "Gallo pinto", "Breakfast.", "Rice, beans", "Fry.", "https://example.org/a.jpg").ImageUrl);
        Assert.Throws<ArgumentOutOfRangeException>(() => Dish.Create(0, "Gallo pinto", "Breakfast.", "Rice, beans", "Fry."));
        Assert.ThrowsAny<ArgumentException>(() => Dish.Create(1, " ", "Breakfast.", "Rice, beans", "Fry."));
        Assert.Throws<ArgumentNullException>(() => Dish.Create(1, "Gallo pinto", null!, "Rice, beans", "Fry."));
        Assert.ThrowsAny<ArgumentException>(() => Dish.Create(1, "Gallo pinto", "Breakfast.", "", "Fry."));
        Assert.ThrowsAny<ArgumentException>(() => Dish.Create(1, "Gallo pinto", "Breakfast.", "Rice, beans", ""));
    }

    [Fact]
    public async Task GetTraditions_MapsEveryTradition()
    {
        var repository = new FakeCulture { Traditions = [Tradition.Create(1, "Día del Boyero", "Ox carts.", "https://example.org/b.jpg")] };

        var result = await new GetTraditionsInteractor(repository).ExecuteAsync(GetTraditionsRequest.Instance, Ct);

        Assert.Equal([new TraditionResponse(1, "Día del Boyero", "Ox carts.", "https://example.org/b.jpg")], result.Value);
    }

    [Fact]
    public async Task GetDishes_MapsEveryDish()
    {
        var repository = new FakeCulture { Dishes = [Dish.Create(1, "Gallo pinto", "Breakfast.", "Rice, beans", "Fry.")] };

        var result = await new GetDishesInteractor(repository).ExecuteAsync(GetDishesRequest.Instance, Ct);

        Assert.Equal([new DishResponse(1, "Gallo pinto", "Breakfast.", "Rice, beans", "Fry.", null)], result.Value);
    }

    [Fact]
    public async Task EmptyCatalogs_AreNotFound()
    {
        var repository = new FakeCulture();

        Assert.Equal(Error.NotFound("No traditions found. Please check later for updates."),
            (await new GetTraditionsInteractor(repository).ExecuteAsync(GetTraditionsRequest.Instance, Ct)).Error);
        Assert.Equal(Error.NotFound("No dishes found. Please check later for updates."),
            (await new GetDishesInteractor(repository).ExecuteAsync(GetDishesRequest.Instance, Ct)).Error);
    }

    private sealed class FakeCulture : ICultureReadRepository
    {
        public List<Tradition> Traditions { get; init; } = [];

        public List<Dish> Dishes { get; init; } = [];

        public Task<IReadOnlyList<Tradition>> GetTraditionsAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Tradition>>(Traditions);

        public Task<IReadOnlyList<Dish>> GetDishesAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Dish>>(Dishes);
    }
}
