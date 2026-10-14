using System.Net;
using DiscoverCostaRica.CharacterizationTests.Harness;
using DiscoverCostaRica.Testing.Containers;

namespace DiscoverCostaRica.CharacterizationTests.Beaches;

/// <summary>Spec 002 / contract § Beaches, as built.</summary>
public sealed class BeachesV1Tests(SeededCatalog catalog, SqlServerFixture sql) : IClassFixture<SeededCatalog>
{
    private const string Role = "Beaches.Read";
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Beaches_ReturnsAllInEnvelope()
    {
        using var client = catalog.Beaches.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/beaches/", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.OK,
            """{"value":[{"id":1,"name":"Manuel Antonio","description":"White sand inside a national park."},{"id":2,"name":"Tamarindo","description":"Surf town on the Nicoya peninsula."}],"statusCode":200}""");
    }

    [Fact]
    public async Task WithoutToken_Returns401()
    {
        using var client = catalog.Beaches.CreateClient();

        var response = await client.GetAsync("/api/v1/beaches/", Ct);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task WithAnotherServiceRole_Returns403()
    {
        using var client = catalog.Beaches.CreateClientWithRoles("Geo.Read");

        var response = await client.GetAsync("/api/v1/beaches/", Ct);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task EmptyCatalog_Returns404WithMessage()
    {
        var connectionString = await CatalogDatabase.CreateAsync(sql, seeded: false, Ct);
        await using var beaches = new BeachesApiFactory(connectionString);
        using var client = beaches.CreateClientWithRoles(Role);

        var response = await client.GetAsync("/api/v1/beaches/", Ct);

        await ResponseAssert.Exactly(response, HttpStatusCode.NotFound,
            """{"statusCode":404,"message":"No beaches found. Please check later for updates."}""");
    }

    [Fact]
    public async Task Beaches_AreServedFromCacheAfterFirstRead()
    {
        var connectionString = await CatalogDatabase.CreateAsync(sql, seeded: true, Ct);
        await using var beaches = new BeachesApiFactory(connectionString);
        using var client = beaches.CreateClientWithRoles(Role);

        var first = await (await client.GetAsync("/api/v1/beaches/", Ct)).Content.ReadAsStringAsync(Ct);
        await SqlServerFixture.ExecuteAsync(connectionString, "DELETE FROM [Beach.Beach];", Ct);
        var second = await client.GetAsync("/api/v1/beaches/", Ct);

        await ResponseAssert.Exactly(second, HttpStatusCode.OK, first);
    }
}
