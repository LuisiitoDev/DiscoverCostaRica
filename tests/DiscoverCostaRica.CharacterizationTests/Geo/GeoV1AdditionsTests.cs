using System.Net;
using System.Net.Http.Json;
using DiscoverCostaRica.CharacterizationTests.Harness;

namespace DiscoverCostaRica.CharacterizationTests.Geo;

/// <summary>Spec 006 US2: additive v1 routes and the fixes for GEO-D2 / GEO-D3 (contract § Geo).</summary>
public sealed class GeoV1AdditionsTests(SeededCatalog catalog) : IClassFixture<SeededCatalog>
{
    private const string Role = "Geo.Read";
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task DeprecatedDistrictsRoute_KeepsAsBuiltResult_AndPointsToItsSuccessor()
    {
        using var client = catalog.Geo.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/geo/districts/1", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.OK,
            """{"value":[{"id":1,"name":"Carmen"},{"id":1,"name":"Alajuela"},{"id":1,"name":"Liberia"},{"id":2,"name":"Merced"}],"statusCode":200}""");
        Assert.Equal("true", response.Headers.GetValues("Deprecation").Single());
        Assert.Equal("""</api/v1/geo/provinces/{provinceId}/cantons/1/districts>; rel="successor-version" """.TrimEnd(), response.Headers.GetValues("Link").Single());
    }

    [Fact]
    [Trait("Fixed", "GEO-D2")]
    public async Task DeprecatedDistrictsRoute_WithProvince_ReturnsOnlyThatCanton()
    {
        using var client = catalog.Geo.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/geo/districts/1?provinceId=1", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.OK,
            """{"value":[{"id":1,"name":"Carmen"},{"id":2,"name":"Merced"}],"statusCode":200}""");
        Assert.Equal("""</api/v1/geo/provinces/1/cantons/1/districts>; rel="successor-version" """.TrimEnd(), response.Headers.GetValues("Link").Single());
    }

    [Fact]
    [Trait("Fixed", "GEO-D3")]
    public async Task DeprecatedDistrictRoute_WithProvince_IsUnambiguous()
    {
        using var client = catalog.Geo.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/geo/districts/1/1?provinceId=2", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.OK, """{"value":{"id":1,"name":"Alajuela"},"statusCode":200}""");
    }

    [Theory]
    [InlineData("/api/v1/geo/provinces/2/cantons", HttpStatusCode.OK, """{"value":[{"id":1,"name":"Alajuela"},{"id":8,"name":"Poás"},{"id":10,"name":"San Carlos"}],"statusCode":200}""")]
    [InlineData("/api/v1/geo/provinces/9/cantons", HttpStatusCode.NotFound, """{"statusCode":404,"message":"No cantons found. Please check later for updates."}""")]
    [InlineData("/api/v1/geo/provinces/2/cantons/10", HttpStatusCode.OK, """{"value":{"id":10,"name":"San Carlos"},"statusCode":200}""")]
    [InlineData("/api/v1/geo/provinces/2/cantons/99", HttpStatusCode.NotFound, """{"statusCode":404,"message":"No canton was found for 99"}""")]
    [InlineData("/api/v1/geo/provinces/1/cantons/1/districts", HttpStatusCode.OK, """{"value":[{"id":1,"name":"Carmen"},{"id":2,"name":"Merced"}],"statusCode":200}""")]
    [InlineData("/api/v1/geo/provinces/1/cantons/2/districts", HttpStatusCode.NotFound, """{"statusCode":404,"message":"No districts found. Please check later for updates."}""")]
    [InlineData("/api/v1/geo/provinces/2/cantons/10/districts/7", HttpStatusCode.OK, """{"value":{"id":7,"name":"La Fortuna"},"statusCode":200}""")]
    [InlineData("/api/v1/geo/provinces/1/cantons/10/districts/7", HttpStatusCode.NotFound, """{"statusCode":404,"message":"No district was found for 7"}""")]
    public async Task HierarchicalRoutes_AreScopedByTheirParents(string route, HttpStatusCode status, string body)
    {
        using var client = catalog.Geo.CreateClientWithRoles(Role);

        var response = await client.GetAsync(route, Ct);

        await ResponseAssert.Exactly(response, status, body);
        Assert.False(response.Headers.Contains("Deprecation"));
    }

    [Fact]
    public async Task ResolveLocations_ResolvesEveryItemInOrder_UnknownLevelsOmitted()
    {
        using var client = catalog.Geo.CreateClientWithRoles(Role);

        var response = await client.PostAsJsonAsync("/api/v1/geo/locations:resolve", new
        {
            items = new object[]
            {
                new { provinceId = 2, cantonId = 10, districtId = 7 },
                new { provinceId = 5, cantonId = 1 },
                new { provinceId = 9, cantonId = 1 },
                new { provinceId = 1, cantonId = 1, districtId = 99 },
            },
        }, Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.OK,
            """{"value":{"items":[{"province":{"id":2,"name":"Alajuela"},"canton":{"id":10,"name":"San Carlos"},"district":{"id":7,"name":"La Fortuna"}},{"province":{"id":5,"name":"Guanacaste"},"canton":{"id":1,"name":"Liberia"}},{},{"province":{"id":1,"name":"San José"},"canton":{"id":1,"name":"San José"}}]},"statusCode":200}""");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(201)]
    public async Task ResolveLocations_RejectsBatchesOutsideOneTo200(int size)
    {
        using var client = catalog.Geo.CreateClientWithRoles(Role);
        var items = Enumerable.Range(0, size).Select(_ => new { provinceId = 1, cantonId = 1 }).ToArray();

        var response = await client.PostAsJsonAsync("/api/v1/geo/locations:resolve", new { items }, Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.BadRequest,
            """{"statusCode":400,"message":"items must contain between 1 and 200 entries"}""");
    }

    [Fact]
    public async Task ResolveLocations_WithoutBody_IsAValidationError()
    {
        using var client = catalog.Geo.CreateClientWithRoles(Role);

        var response = await client.PostAsync("/api/v1/geo/locations:resolve", content: null, Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.BadRequest,
            """{"statusCode":400,"message":"items must contain between 1 and 200 entries"}""");
    }

    [Fact]
    public async Task ResolveLocations_NeedsTheGeoRole()
    {
        using var client = catalog.Geo.CreateClientWithRoles("Volcano.Read");

        var response = await client.PostAsJsonAsync("/api/v1/geo/locations:resolve", new { items = new[] { new { provinceId = 1, cantonId = 1 } } }, Ct);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
