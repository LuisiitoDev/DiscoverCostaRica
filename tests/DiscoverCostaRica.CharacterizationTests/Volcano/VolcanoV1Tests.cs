using System.Net;
using DiscoverCostaRica.CharacterizationTests.Harness;
using DiscoverCostaRica.Testing.Containers;

namespace DiscoverCostaRica.CharacterizationTests.Volcano;

/// <summary>Spec 004 / contract § Volcano, as built. Geo runs in-process behind Volcano's HTTP client.</summary>
public sealed class VolcanoV1Tests(SeededCatalog catalog, SqlServerFixture sql) : IClassFixture<SeededCatalog>
{
    private const string Role = "Volcano.Read";
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    [Trait("Defect", "VOL-D4")]
    public async Task Volcanoes_ReturnAllWithLocationResolvedThroughGeo()
    {
        using var client = catalog.Volcano.CreateClientWithRoles(Role);

        // As built this costs up to 3 Geo calls per volcano (N+1, VOL-D4); the payload is the contract.
        var response = await client.GetAsync("/api/v1/volcanoes/", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.OK,
            """{"value":[{"id":1,"name":"Arenal","description":"Young stratovolcano near La Fortuna.","location":{"province":{"id":2,"name":"Alajuela"},"canton":{"id":10,"name":"San Carlos"},"district":{"id":7,"name":"La Fortuna"}}},{"id":2,"name":"Poás","description":"Active volcano with an acidic crater lake.","location":{"province":{"id":2,"name":"Alajuela"},"canton":{"id":8,"name":"Poás"}}},{"id":3,"name":"Rincón de la Vieja","description":"Complex volcano in Guanacaste.","location":{"province":{"id":5,"name":"Guanacaste"},"canton":{"id":1,"name":"Liberia"}}}],"statusCode":200}""");
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
    [Trait("Defect", "VOL-D1")]
    public async Task VolcanoById_AsBuilt_IgnoresThePathAndNeedsAQueryId()
    {
        using var client = catalog.Volcano.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/volcanoes/1", Ct);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    [Trait("Defect", "VOL-D5")]
    public async Task VolcanoById_WithQueryId_ReturnsVolcanoWithoutLocation()
    {
        using var client = catalog.Volcano.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/volcanoes/1?id=1", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.OK,
            """{"value":{"id":1,"name":"Arenal","description":"Young stratovolcano near La Fortuna."},"statusCode":200}""");
    }

    [Fact]
    public async Task UnknownVolcano_Returns404WithMessage()
    {
        using var client = catalog.Volcano.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/volcanoes/99?id=99", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.NotFound,
            """{"statusCode":404,"message":"Volcano was not found"}""");
    }

    [Fact]
    [Trait("Defect", "VOL-D1")]
    public async Task VolcanoesByProvince_AsBuilt_ReadTheQueryIdNotThePath()
    {
        using var client = catalog.Volcano.CreateClientWithRoles(Role);

        // Path says province 5 (Guanacaste); the query wins and returns province 2.
        var response = await client.GetAsync("/api/v1/volcanoes/province/5?id=2", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.OK,
            """{"value":[{"id":1,"name":"Arenal","description":"Young stratovolcano near La Fortuna."},{"id":2,"name":"Poás","description":"Active volcano with an acidic crater lake."}],"statusCode":200}""");
    }

    [Fact]
    public async Task VolcanoesOfProvinceWithoutVolcanoes_Returns404WithMessage()
    {
        using var client = catalog.Volcano.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/volcanoes/province/1?id=1", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.NotFound,
            """{"statusCode":404,"message":"Volcanos were not found"}""");
    }

    [Fact]
    [Trait("Defect", "VOL-D2")]
    public async Task VolcanoRoutes_AsBuilt_AnswerAnyHttpVerb()
    {
        using var client = catalog.Volcano.CreateClientWithRoles(Role);

        var response = await client.DeleteAsync("/api/v1/volcanoes/1?id=1", Ct);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task WithoutToken_Returns401()
    {
        using var client = catalog.Volcano.CreateClient();

        var response = await client.GetAsync("/api/v1/volcanoes/1?id=1", Ct);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task WithAnotherServiceRole_Returns403()
    {
        using var client = catalog.Volcano.CreateClientWithRoles("Geo.Read");

        var response = await client.GetAsync("/api/v1/volcanoes/1?id=1", Ct);

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

        await ResponseAssert.Exactly(response, HttpStatusCode.NotFound,
            """{"statusCode":404,"message":"Volcanos were not found"}""");
    }
}
