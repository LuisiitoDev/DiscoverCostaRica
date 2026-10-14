using System.Net;
using System.Text;
using System.Text.Json;
using DiscoverCostaRica.BuildingBlocks.Infrastructure.Authentication;
using DiscoverCostaRica.Volcano.Application.Ports;
using DiscoverCostaRica.Volcano.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace DiscoverCostaRica.Volcano.UnitTests;

/// <summary>Edge cases of the Geo gateway; the happy path and outages run end to end in the characterization suite.</summary>
public sealed class GeoLocationGatewayTests
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task MoreCodesThanOneBatch_AreSplitIntoBatchesOf200()
    {
        var geo = new FakeGeoHandler(request => Answer(request, (2, "Alajuela")));
        var gateway = Build(geo);
        var codes = Enumerable.Range(1, 250).Select(c => LocationCode.Create(2, c)).ToList();

        var resolved = await gateway.ResolveAsync(codes, Ct);

        Assert.Equal(250, resolved.Count);
        Assert.Equal([200, 50], geo.BatchSizes);
        Assert.All(geo.Tokens, t => Assert.Equal("Bearer s2s-token", t));
    }

    [Fact]
    public async Task ResolvedCodes_AreCached_SoGeoIsNotCalledAgain()
    {
        var geo = new FakeGeoHandler(request => Answer(request, (2, "Alajuela")));
        var gateway = Build(geo);
        var codes = new[] { LocationCode.Create(2, 10, 7) };

        await gateway.ResolveAsync(codes, Ct);
        var again = await gateway.ResolveAsync(codes, Ct);

        Assert.Equal("Alajuela", again[codes[0]].Province!.Name);
        Assert.Single(geo.BatchSizes);
    }

    [Fact]
    public async Task CodesGeoDoesNotKnow_AreLeftOut()
    {
        var gateway = Build(new FakeGeoHandler(request => Answer(request, province: null)));

        var resolved = await gateway.ResolveAsync([LocationCode.Create(7, 99)], Ct);

        Assert.Empty(resolved);
    }

    [Theory]
    [InlineData("""{"statusCode":200}""")]
    [InlineData("not json")]
    public async Task UnexpectedAnswers_AreTreatedAsUnavailable(string body)
    {
        var gateway = Build(new FakeGeoHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json"),
        }));

        var resolved = await gateway.ResolveAsync([LocationCode.Create(2, 10)], Ct);

        Assert.Empty(resolved);
    }

    [Fact]
    public async Task NothingToResolve_DoesNotCallGeo()
    {
        var geo = new FakeGeoHandler(request => Answer(request, (2, "Alajuela")));

        var resolved = await Build(geo).ResolveAsync([], Ct);

        Assert.Empty(resolved);
        Assert.Empty(geo.BatchSizes);
    }

    [Fact]
    public async Task CallerCancellation_IsNotSwallowed()
    {
        using var cancelled = new CancellationTokenSource();
        await cancelled.CancelAsync();
        var gateway = Build(new FakeGeoHandler(request => Answer(request, (2, "Alajuela"))));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => gateway.ResolveAsync([LocationCode.Create(2, 10)], cancelled.Token));
    }

    private static IGeoLocationGateway Build(FakeGeoHandler geo)
    {
        var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IAccessTokenProvider, StaticToken>();
        services.AddHybridCache();
        services.AddTransient<BearerTokenHandler>();
        services.AddGeoLocationGateway();
        services.ConfigureAll<HttpClientFactoryOptions>(o => o.HttpMessageHandlerBuilderActions.Add(b => b.PrimaryHandler = geo));
        return services.BuildServiceProvider().GetRequiredService<IGeoLocationGateway>();
    }

    private static HttpResponseMessage Answer(HttpRequestMessage request, (int Id, string Name)? province)
    {
        using var body = JsonDocument.Parse(request.Content!.ReadAsStringAsync().GetAwaiter().GetResult());
        var count = body.RootElement.GetProperty("items").GetArrayLength();
        var item = province is { } p ? $$$"""{"province":{"id":{{{p.Id}}},"name":"{{{p.Name}}}"}}""" : "{}";
        var json = $$$"""{"value":{"items":[{{{string.Join(',', Enumerable.Repeat(item, count))}}}]},"statusCode":200}""";
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json, Encoding.UTF8, "application/json") };
    }

    private sealed class StaticToken : IAccessTokenProvider
    {
        public Task<string> GetTokenAsync(CancellationToken cancellationToken) => Task.FromResult("s2s-token");
    }

    private sealed class FakeGeoHandler(Func<HttpRequestMessage, HttpResponseMessage> answer) : HttpMessageHandler
    {
        public List<int> BatchSizes { get; } = [];

        public List<string> Tokens { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var body = JsonDocument.Parse(await request.Content!.ReadAsStringAsync(cancellationToken));
            BatchSizes.Add(body.RootElement.GetProperty("items").GetArrayLength());
            Tokens.Add(request.Headers.Authorization!.ToString());
            return answer(request);
        }
    }
}
