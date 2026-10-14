using System.Net;
using DiscoverCostaRica.CharacterizationTests.Harness;
using DiscoverCostaRica.Testing.Containers;

namespace DiscoverCostaRica.CharacterizationTests.Volcano;

/// <summary>Spec 004 / contract § Volcano. Geo runs in-process behind Volcano's HTTP client.</summary>
public sealed class VolcanoV1Tests(SeededCatalog catalog, SqlServerFixture sql) : IClassFixture<SeededCatalog>
{
    private const string Role = "Volcano.Read";
    private const string Arenal = """{"id":1,"name":"Arenal","description":"Young stratovolcano near La Fortuna.","location":{"province":{"id":2,"name":"Alajuela"},"canton":{"id":10,"name":"San Carlos"},"district":{"id":7,"name":"La Fortuna"}}}""";
    private const string Poas = """{"id":2,"name":"Poás","description":"Active volcano with an acidic crater lake.","location":{"province":{"id":2,"name":"Alajuela"},"canton":{"id":8,"name":"Poás"}}}""";
    private const string Rincon = """{"id":3,"name":"Rincón de la Vieja","description":"Complex volcano in Guanacaste.","location":{"province":{"id":5,"name":"Guanacaste"},"canton":{"id":1,"name":"Liberia"}}}""";

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    [Trait("Fixed", "VOL-D4")]
    public async Task Volcanoes_ReturnAllWithLocation_InOneGeoCall()
    {
        await using var volcano = await NewVolcanoAsync();
        using var client = volcano.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/volcanoes/", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.OK, $$"""{"value":[{{Arenal}},{{Poas}},{{Rincon}}],"statusCode":200}""");
        Assert.Equal(1, volcano.GeoCalls);
    }

    [Fact]
    public async Task Volcanoes_AreServedFromCacheAfterFirstRead()
    {
        var connectionString = await CatalogDatabase.CreateAsync(sql, seeded: true, Ct);
        await using var geo = new GeoApiFactory(connectionString);
        await using var volcano = new VolcanoApiFactory(connectionString, geo);
        using var client = volcano.CreateClientWithRoles(Role);

        var first = await (await client.GetAsync("/api/v1/volcanoes/", Ct)).Content.ReadAsStringAsync(Ct);
        await SqlServerFixture.ExecuteAsync(connectionString, "DELETE FROM [Volcano.Volcano];", Ct);
        var second = await client.GetAsync("/api/v1/volcanoes/", Ct);

        await ResponseAssert.Exactly(second, HttpStatusCode.OK, first);
    }

    [Fact]
    [Trait("Fixed", "VOL-D1")]
    [Trait("Fixed", "VOL-D5")]
    public async Task VolcanoById_ReadsThePath_AndIncludesTheLocation()
    {
        using var client = catalog.Volcano.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/volcanoes/1", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.OK, $$"""{"value":{{Arenal}},"statusCode":200}""");
    }

    [Fact]
    public async Task UnknownVolcano_Returns404WithMessage()
    {
        using var client = catalog.Volcano.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/volcanoes/99", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.NotFound, """{"statusCode":404,"message":"Volcano was not found"}""");
    }

    [Fact]
    [Trait("Fixed", "VOL-D1")]
    [Trait("Fixed", "VOL-D5")]
    public async Task VolcanoesByProvince_ReadThePath_AndIncludeLocations()
    {
        using var client = catalog.Volcano.CreateClientWithRoles(Role);

        // A leftover ?id= from old clients no longer wins over the path.
        var response = await client.GetAsync("/api/v1/volcanoes/province/2?id=5", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.OK, $$"""{"value":[{{Arenal}},{{Poas}}],"statusCode":200}""");
    }

    [Fact]
    public async Task VolcanoesOfProvinceWithoutVolcanoes_Returns404WithMessage()
    {
        using var client = catalog.Volcano.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/volcanoes/province/1", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.NotFound, """{"statusCode":404,"message":"Volcanos were not found"}""");
    }

    [Theory]
    [Trait("Fixed", "VOL-D2")]
    [InlineData("DELETE", "/api/v1/volcanoes/1")]
    [InlineData("POST", "/api/v1/volcanoes/")]
    [InlineData("PUT", "/api/v1/volcanoes/province/2")]
    public async Task VolcanoRoutes_AreReadOnly(string method, string route)
    {
        using var client = catalog.Volcano.CreateClientWithRoles(Role);

        var response = await client.SendAsync(new HttpRequestMessage(new HttpMethod(method), route), Ct);

        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }

    [Fact]
    public async Task GeoDown_VolcanoesKeepTheLocationsResolvedBefore()
    {
        await using var volcano = await NewVolcanoAsync();
        using var client = volcano.CreateClientWithRoles(Role);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/v1/volcanoes/", Ct)).StatusCode);

        volcano.GeoAvailable = false;
        var response = await client.GetAsync("/api/v1/volcanoes/1", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.OK, $$"""{"value":{{Arenal}},"statusCode":200}""");
    }

    [Fact]
    public async Task GeoDown_VolcanoesAreServedWithoutLocation_NotAnError()
    {
        await using var volcano = await NewVolcanoAsync();
        volcano.GeoAvailable = false;
        using var client = volcano.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/volcanoes/2", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.OK,
            """{"value":{"id":2,"name":"Poás","description":"Active volcano with an acidic crater lake."},"statusCode":200}""");
    }

    [Fact]
    public async Task WithoutToken_Returns401()
    {
        using var client = catalog.Volcano.CreateClient();

        var response = await client.GetAsync("/api/v1/volcanoes/1", Ct);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task WithAnotherServiceRole_Returns403()
    {
        using var client = catalog.Volcano.CreateClientWithRoles("Geo.Read");

        var response = await client.GetAsync("/api/v1/volcanoes/1", Ct);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task EmptyCatalog_Volcanoes_Returns404WithMessage()
    {
        var connectionString = await CatalogDatabase.CreateAsync(sql, seeded: false, Ct);
        await using var geo = new GeoApiFactory(connectionString);
        await using var volcano = new VolcanoApiFactory(connectionString, geo);
        using var client = volcano.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/volcanoes/", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.NotFound, """{"statusCode":404,"message":"Volcanos were not found"}""");
    }

    /// <summary>A Volcano with its own cache, over the shared seeded database.</summary>
    private Task<VolcanoApiFactory> NewVolcanoAsync() =>
        Task.FromResult(new VolcanoApiFactory(catalog.ConnectionString, catalog.Geo));
}
