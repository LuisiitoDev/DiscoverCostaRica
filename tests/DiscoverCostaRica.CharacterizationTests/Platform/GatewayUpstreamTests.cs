using System.Net;
using DiscoverCostaRica.CharacterizationTests.Harness;

namespace DiscoverCostaRica.CharacterizationTests.Platform;

/// <summary>
/// Spec 006 US5: every upstream path the gateway forwards to (GatewayRoutesTests) is a working route
/// of its service, exactly as forwarded (no trailing slash), so no public path dead-ends in a 404 (BCH-D1, VOL-D7).
/// </summary>
public sealed class GatewayUpstreamTests(SeededCatalog catalog) : IClassFixture<SeededCatalog>
{
    public static TheoryData<string, string> Upstreams => new()
    {
        { "geo", "/api/v1/geo/provinces" },
        { "geo", "/api/v1/geo/provinces/2/cantons/10" },
        { "geo", "/api/v1/geo/cantons/1" },
        { "geo", "/api/v1/geo/cantons/1/2" },
        { "geo", "/api/v1/geo/districts/1/1" },
        { "beaches", "/api/v1/beaches" },
        { "culture", "/api/v1/traditions/tradition" },
        { "culture", "/api/v1/traditions/dish" },
        { "volcano", "/api/v1/volcanoes" },
        { "volcano", "/api/v1/volcanoes/1" },
        { "volcano", "/api/v1/volcanoes/province/2" },
    };

    [Theory]
    [MemberData(nameof(Upstreams))]
    public async Task ForwardedPath_IsServed(string service, string path)
    {
        using var client = service switch
        {
            "geo" => catalog.Geo.CreateClientWithRoles("Geo.Read"),
            "beaches" => catalog.Beaches.CreateClientWithRoles("Beaches.Read"),
            "culture" => catalog.Culture.CreateClientWithRoles("Culture.Read"),
            _ => catalog.Volcano.CreateClientWithRoles("Volcano.Read"),
        };

        var response = await client.GetAsync(path, TestContext.Current.CancellationToken);

        await ResponseAssert.Status(response, HttpStatusCode.OK);
    }
}
