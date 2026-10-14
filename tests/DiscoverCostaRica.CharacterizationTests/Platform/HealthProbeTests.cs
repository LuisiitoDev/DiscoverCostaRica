using System.Net;
using DiscoverCostaRica.CharacterizationTests.Harness;

namespace DiscoverCostaRica.CharacterizationTests.Platform;

/// <summary>Spec 006 US7 (PLT-D6): probes answer in production, without a token, and leak nothing.</summary>
public sealed class HealthProbeTests(SeededCatalog catalog) : IClassFixture<SeededCatalog>
{
    public static TheoryData<string, string> Probes => new()
    {
        { "geo", "/alive" }, { "geo", "/health" },
        { "beaches", "/alive" }, { "beaches", "/health" },
        { "culture", "/alive" }, { "culture", "/health" },
        { "volcano", "/alive" }, { "volcano", "/health" },
    };

    [Theory]
    [MemberData(nameof(Probes))]
    public async Task Probes_AreAnonymous_AndReportOnlyTheStatus(string service, string probe)
    {
        using var client = service switch
        {
            "geo" => catalog.Geo.CreateClient(),
            "beaches" => catalog.Beaches.CreateClient(),
            "culture" => catalog.Culture.CreateClient(),
            _ => catalog.Volcano.CreateClient(),
        };

        var response = await client.GetAsync(probe, TestContext.Current.CancellationToken);

        await ResponseAssert.Exactly(response, HttpStatusCode.OK, "Healthy");
    }
}
