using System.Net;
using DiscoverCostaRica.CharacterizationTests.Harness;
using DiscoverCostaRica.Testing.Containers;

namespace DiscoverCostaRica.CharacterizationTests.Culture;

/// <summary>Spec 003 / contract § Culture, as built.</summary>
public sealed class CultureV1Tests(SeededCatalog catalog, SqlServerFixture sql) : IClassFixture<SeededCatalog>
{
    private const string Role = "Culture.Read";
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Traditions_ReturnsAllInEnvelope_NullImageOmitted()
    {
        using var client = catalog.Culture.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/traditions/tradition", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.OK,
            """{"value":[{"id":1,"name":"Día del Boyero","description":"Parade of painted ox carts.","imageUrl":"https://example.org/boyero.jpg"},{"id":2,"name":"Fiestas de Palmares","description":"January festival with tope and music."}],"statusCode":200}""");
    }

    [Fact]
    public async Task Dishes_ReturnsAllInEnvelope()
    {
        using var client = catalog.Culture.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/traditions/dish", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.OK,
            """{"value":[{"id":1,"name":"Gallo pinto","description":"Rice and beans breakfast.","ingredients":"Rice, black beans, onion, cilantro","preparation":"Fry the rice with the beans.","imageUrl":"https://example.org/gallo-pinto.jpg"}],"statusCode":200}""");
    }

    [Fact]
    public async Task WithoutToken_Returns401()
    {
        using var client = catalog.Culture.CreateClient();

        var response = await client.GetAsync("/api/v1/traditions/dish", Ct);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task WithAnotherServiceRole_Returns403()
    {
        using var client = catalog.Culture.CreateClientWithRoles("Volcano.Read");

        var response = await client.GetAsync("/api/v1/traditions/tradition", Ct);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/v1/traditions/tradition", "No traditions found. Please check later for updates.")]
    [InlineData("/api/v1/traditions/dish", "No dishes found. Please check later for updates.")]
    public async Task EmptyCatalog_Returns404WithMessage(string route, string message)
    {
        var connectionString = await CatalogDatabase.CreateAsync(sql, seeded: false, Ct);
        await using var culture = new CultureApiFactory(connectionString);
        using var client = culture.CreateClientWithRoles(Role);

        var response = await client.GetAsync(route, Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.NotFound, $$"""{"statusCode":404,"message":"{{message}}"}""");
    }

    [Fact]
    public async Task TraditionsAndDishes_AreServedFromCacheAfterFirstRead()
    {
        var connectionString = await CatalogDatabase.CreateAsync(sql, seeded: true, Ct);
        await using var culture = new CultureApiFactory(connectionString);
        using var client = culture.CreateClientWithRoles(Role);

        var traditions = await (await client.GetAsync("/api/v1/traditions/tradition", Ct)).Content.ReadAsStringAsync(Ct);
        var dishes = await (await client.GetAsync("/api/v1/traditions/dish", Ct)).Content.ReadAsStringAsync(Ct);
        await SqlServerFixture.ExecuteAsync(connectionString, "DELETE FROM [Culture.Tradition]; DELETE FROM [Culture.Dish];", Ct);

        await ResponseAssert.Exactly(await client.GetAsync("/api/v1/traditions/tradition", Ct), HttpStatusCode.OK, traditions);
        await ResponseAssert.Exactly(await client.GetAsync("/api/v1/traditions/dish", Ct), HttpStatusCode.OK, dishes);
    }
}
