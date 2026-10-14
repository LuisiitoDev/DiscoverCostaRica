using System.Net;
using DiscoverCostaRica.BuildingBlocks.Infrastructure.Authentication;
using DiscoverCostaRica.BuildingBlocks.Web.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace DiscoverCostaRica.BuildingBlocks.UnitTests.Infrastructure;

public sealed class AuthenticationTests
{
    [Fact]
    public async Task BearerTokenHandler_AddsTheServiceToken()
    {
        var inner = new CapturingHandler();
        using var client = new HttpClient(new BearerTokenHandler(new FixedToken("abc")) { InnerHandler = inner });

        await client.GetAsync(new Uri("https://geoservice/api"), TestContext.Current.CancellationToken);

        Assert.Equal("Bearer abc", inner.Authorization);
    }

    [Fact]
    public void ClientCredentialsTokenProvider_IsASingletonBoundToTheAzureSection()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Azure:TenantId"] = "00000000-0000-0000-0000-000000000001",
            ["Azure:ClientId"] = "00000000-0000-0000-0000-000000000002",
            ["Azure:ClientSecret"] = "secret",
            ["Azure:Scope"] = "api://geo/.default",
        });

        builder.AddClientCredentialsTokenProvider();
        using var host = builder.Build();

        var first = host.Services.GetRequiredService<IAccessTokenProvider>();
        Assert.IsType<ClientCredentialsTokenProvider>(first);
        Assert.Same(first, host.Services.GetRequiredService<IAccessTokenProvider>());
        Assert.Equal("api://geo/.default", host.Services.GetRequiredService<IOptions<ClientCredentialsOptions>>().Value.Scope);
    }

    [Fact]
    public void EntraIdAuthentication_RequiresItsConfigurationSection()
    {
        var builder = Host.CreateApplicationBuilder();

        Assert.Throws<InvalidOperationException>(() => builder.AddEntraIdAuthentication());
    }

    [Theory]
    [InlineData("Development", false)]
    [InlineData("Production", true)]
    public void EntraIdAuthentication_RequiresHttpsMetadataOutsideDevelopment(string environment, bool required)
    {
        // A web host like the services use; Development turns on container validation, which needs routing.
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = environment });
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["EntraId:Instance"] = "https://login.microsoftonline.com/",
            ["EntraId:TenantId"] = "00000000-0000-0000-0000-000000000001",
            ["EntraId:ClientId"] = "00000000-0000-0000-0000-000000000002",
            ["EntraId:Audience"] = "api://discover-costa-rica",
        });

        builder.AddEntraIdAuthentication();
        using var host = builder.Build();

        var options = host.Services.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>().Get(JwtBearerDefaults.AuthenticationScheme);
        Assert.Equal(required, options.RequireHttpsMetadata);
        Assert.Equal(["api://discover-costa-rica"], options.TokenValidationParameters.ValidAudiences);
        Assert.IsType<DiscoverCostaRicaBearerEvents>(options.Events);
    }

    [Fact]
    public async Task BearerEvents_LogFailuresAndChallenges_WithoutChangingTheOutcome()
    {
        var services = new ServiceCollection().AddLogging().BuildServiceProvider();
        var http = new DefaultHttpContext { RequestServices = services };
        var scheme = new AuthenticationScheme(JwtBearerDefaults.AuthenticationScheme, null, typeof(JwtBearerHandler));
        var options = new JwtBearerOptions();
        var events = new DiscoverCostaRicaBearerEvents();

        await events.AuthenticationFailed(new AuthenticationFailedContext(http, scheme, options) { Exception = new InvalidOperationException("bad token") });
        await events.Challenge(new JwtBearerChallengeContext(http, scheme, options, new AuthenticationProperties()));
        await events.TokenValidated(new TokenValidatedContext(http, scheme, options));

        Assert.Equal(StatusCodes.Status200OK, http.Response.StatusCode);
    }

    private sealed class FixedToken(string token) : IAccessTokenProvider
    {
        public Task<string> GetTokenAsync(CancellationToken cancellationToken) => Task.FromResult(token);
    }

    private sealed class CapturingHandler : HttpMessageHandler
    {
        public string? Authorization { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Authorization = request.Headers.Authorization?.ToString();
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
