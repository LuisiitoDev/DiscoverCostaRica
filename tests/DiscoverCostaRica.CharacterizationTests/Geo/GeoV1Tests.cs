using System.Net;
using DiscoverCostaRica.CharacterizationTests.Harness;
using DiscoverCostaRica.Testing.Containers;

namespace DiscoverCostaRica.CharacterizationTests.Geo;

/// <summary>Spec 001 / contract § Geo, as built.</summary>
public sealed class GeoV1Tests(SeededCatalog catalog, SqlServerFixture sql) : IClassFixture<SeededCatalog>
{
    private const string Role = "Geo.Read";
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Provinces_ReturnsAllInEnvelope()
    {
        using var client = catalog.Geo.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/geo/provinces", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.OK,
            """{"value":[{"id":1,"name":"San José"},{"id":2,"name":"Alajuela"},{"id":5,"name":"Guanacaste"}],"statusCode":200}""");
    }

    [Fact]
    public async Task ProvinceById_ReturnsProvince()
    {
        using var client = catalog.Geo.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/geo/provinces/1", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.OK,
            """{"value":{"id":1,"name":"San José"},"statusCode":200}""");
    }

    [Fact]
    [Trait("Fixed", "GEO-D4")]
    public async Task UnknownProvince_Returns404WithMessage()
    {
        using var client = catalog.Geo.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/geo/provinces/9", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.NotFound,
            """{"statusCode":404,"message":"No province was found for 9"}""");
    }

    [Fact]
    public async Task CantonsOfProvince_ReturnsOnlyThatProvince()
    {
        using var client = catalog.Geo.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/geo/cantons/1", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.OK,
            """{"value":[{"id":1,"name":"San José"},{"id":2,"name":"Escazú"}],"statusCode":200}""");
    }

    [Fact]
    public async Task CantonsOfUnknownProvince_Returns404WithMessage()
    {
        using var client = catalog.Geo.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/geo/cantons/9", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.NotFound,
            """{"statusCode":404,"message":"No cantons found. Please check later for updates."}""");
    }

    [Fact]
    public async Task CantonById_ReturnsCanton()
    {
        using var client = catalog.Geo.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/geo/cantons/2/10", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.OK,
            """{"value":{"id":10,"name":"San Carlos"},"statusCode":200}""");
    }

    [Fact]
    public async Task UnknownCanton_Returns404WithMessage()
    {
        using var client = catalog.Geo.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/geo/cantons/1/99", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.NotFound,
            """{"statusCode":404,"message":"No canton was found for 99"}""");
    }

    [Fact]
    [Trait("Defect", "GEO-D2")]
    public async Task DistrictsOfCanton_AsBuilt_MixDistrictsFromEveryProvince()
    {
        using var client = catalog.Geo.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/geo/districts/1", Ct);

        // Carmen and Merced belong to San José/San José; Alajuela and Liberia leak in from provinces 2 and 5.
        await ResponseAssert.Exactly(response, HttpStatusCode.OK,
            """{"value":[{"id":1,"name":"Carmen"},{"id":1,"name":"Alajuela"},{"id":1,"name":"Liberia"},{"id":2,"name":"Merced"}],"statusCode":200}""");
    }

    [Fact]
    public async Task DistrictsOfUnknownCanton_Returns404WithMessage()
    {
        using var client = catalog.Geo.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/geo/districts/99", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.NotFound,
            """{"statusCode":404,"message":"No districts found. Please check later for updates."}""");
    }

    [Fact]
    [Trait("Defect", "GEO-D3")]
    public async Task DistrictById_AsBuilt_IsAmbiguousAcrossProvinces()
    {
        using var client = catalog.Geo.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/geo/districts/1/1", Ct);

        var body = await ResponseAssert.Status(response, HttpStatusCode.OK);
        Assert.Contains(body, new[]
        {
            """{"value":{"id":1,"name":"Carmen"},"statusCode":200}""",
            """{"value":{"id":1,"name":"Alajuela"},"statusCode":200}""",
            """{"value":{"id":1,"name":"Liberia"},"statusCode":200}""",
        });
    }

    [Fact]
    public async Task UnknownDistrict_Returns404WithMessage()
    {
        using var client = catalog.Geo.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/geo/districts/1/99", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.NotFound,
            """{"statusCode":404,"message":"No district was found for 99"}""");
    }

    [Fact]
    public async Task WithoutToken_Returns401()
    {
        using var client = catalog.Geo.CreateClient();

        var response = await client.GetAsync("/api/v1/geo/provinces", Ct);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task WithAnotherServiceRole_Returns403()
    {
        using var client = catalog.Geo.CreateClientWithRoles("Beaches.Read");

        var response = await client.GetAsync("/api/v1/geo/provinces", Ct);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task EmptyCatalog_Provinces_Returns404WithMessage()
    {
        var connectionString = await CatalogDatabase.CreateAsync(sql, seeded: false, Ct);
        await using var geo = new GeoApiFactory(connectionString);
        using var client = geo.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/geo/provinces", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.NotFound,
            """{"statusCode":404,"message":"No provinces found. Please check later for updates."}""");
    }

    [Fact]
    public async Task Provinces_AreServedFromCacheAfterFirstRead()
    {
        var connectionString = await CatalogDatabase.CreateAsync(sql, seeded: true, Ct);
        await using var geo = new GeoApiFactory(connectionString);
        using var client = geo.CreateClientWithRoles(Role);

        var first = await (await client.GetAsync("/api/v1/geo/provinces", Ct)).Content.ReadAsStringAsync(Ct);
        await SqlServerFixture.ExecuteAsync(connectionString, "DELETE FROM [Geo.District]; DELETE FROM [Geo.Canton]; DELETE FROM [Geo.Province];", Ct);
        var second = await client.GetAsync("/api/v1/geo/provinces", Ct);

        await ResponseAssert.Exactly(second, HttpStatusCode.OK, first);
    }
}
