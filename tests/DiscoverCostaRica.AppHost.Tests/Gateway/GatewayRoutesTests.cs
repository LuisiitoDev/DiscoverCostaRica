using DiscoverCostaRica.AppHost.Constants;
using DiscoverCostaRica.AppHost.Gateway;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.AspNetCore.Routing.Template;

namespace DiscoverCostaRica.AppHost.Tests.Gateway;

/// <summary>
/// Spec 006 US5: the public route table (contracts/public-api-v1.md § Gateway). Paths are resolved with
/// the same routing primitives YARP's PathRouteValues transform uses, so no Aspire host is needed.
/// The upstream paths are the v1 routes the characterization suite exercises against each service.
/// </summary>
[Trait("Category", "Gateway")]
public sealed class GatewayRoutesTests
{
    private static readonly TemplateBinderFactory Binders = new ServiceCollection()
        .AddRouting()
        .BuildServiceProvider()
        .GetRequiredService<TemplateBinderFactory>();

    [Theory]
    [InlineData("/provinces", Microservices.Geo, "/api/v1/geo/provinces")]
    [InlineData("/provinces/2/cantons/10", Microservices.Geo, "/api/v1/geo/provinces/2/cantons/10")]
    [InlineData("/cantons/1", Microservices.Geo, "/api/v1/geo/cantons/1")]
    [InlineData("/canton/1/2", Microservices.Geo, "/api/v1/geo/cantons/1/2")]
    [InlineData("/districts/1/1", Microservices.Geo, "/api/v1/geo/districts/1/1")]
    [InlineData("/beaches", Microservices.Beaches, "/api/v1/beaches")]
    [InlineData("/tradition", Microservices.Culture, "/api/v1/traditions/tradition")]
    [InlineData("/dish", Microservices.Culture, "/api/v1/traditions/dish")]
    [InlineData("/volcanoes", Microservices.Volcano, "/api/v1/volcanoes")]
    [InlineData("/volcanoes/1", Microservices.Volcano, "/api/v1/volcanoes/1")]
    [InlineData("/volcano/1", Microservices.Volcano, "/api/v1/volcanoes/1")]
    [InlineData("/province/2", Microservices.Volcano, "/api/v1/volcanoes/province/2")]
    public void PublicPath_IsForwardedToTheV1Route(string publicPath, string service, string upstream)
    {
        var (route, forwarded) = Assert.Single(Resolve(publicPath));

        Assert.Equal(service, route.Service);
        Assert.Equal(upstream, forwarded);
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/does-not-exist")]
    [InlineData("/api/v1/volcanoes")]
    [InlineData("/provincesX")]
    public void UnknownPaths_AreNotForwarded(string publicPath) =>
        Assert.Empty(Resolve(publicPath));

    [Fact]
    public void EveryRoute_TargetsTheV1Api() =>
        Assert.All(GatewayRoutes.All, route => Assert.StartsWith("/api/v1/", route.Upstream, StringComparison.Ordinal));

    private static List<(GatewayRoute Route, string Upstream)> Resolve(string publicPath)
    {
        var matches = new List<(GatewayRoute, string)>();
        foreach (var route in GatewayRoutes.All)
        {
            var values = new RouteValueDictionary();
            var matcher = new TemplateMatcher(TemplateParser.Parse(route.Path), new RouteValueDictionary());
            if (!matcher.TryMatch(new PathString(publicPath), values))
                continue;

            var upstream = Binders.Create(RoutePatternFactory.Parse(route.Upstream)).BindValues(values)!;
            matches.Add((route, upstream));
        }

        return matches;
    }
}
