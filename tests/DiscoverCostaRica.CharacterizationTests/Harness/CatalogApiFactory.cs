using System.Net.Http.Headers;
using DiscoverCostaRica.Testing.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace DiscoverCostaRica.CharacterizationTests.Harness;

/// <summary>
/// Hosts one catalog service in-process with production settings, a real database,
/// fake authentication and in-memory replacements for the remote infrastructure.
/// </summary>
public class CatalogApiFactory<TMarker>(string connectionString) : WebApplicationFactory<TMarker>
    where TMarker : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // The v1 contract is what production serves (e.g. bad-request handling differs in Development).
        builder.UseEnvironment("Production");

        foreach (var (key, value) in Settings(connectionString))
            builder.UseSetting(key, value);

        builder.ConfigureTestServices(services =>
        {
            services.AddTestAuthentication();
            InfrastructureSeams.Replace(services);
            ConfigureServices(services);
        });
    }

    /// <summary>Extra per-service test wiring.</summary>
    protected virtual void ConfigureServices(IServiceCollection services)
    {
    }

    public HttpClient CreateClientWithRoles(params string[] roles)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization =
            AuthenticationHeaderValue.Parse($"Bearer {TestAuthHandler.TokenFor(roles)}");
        return client;
    }

    private static IEnumerable<(string Key, string Value)> Settings(string connectionString) =>
    [
        ("ConnectionStrings:DiscoverCostaRica", connectionString),
        // Never reached: the Redis-backed cache is replaced in InfrastructureSeams.
        ("ConnectionStrings:redis", "localhost:6390,abortConnect=false,connectTimeout=100"),
        ("Aspire:StackExchange:Redis:DisableHealthChecks", "true"),
        ("Aspire:StackExchange:Redis:DisableTracing", "true"),
        ("EntraId:Instance", "https://login.microsoftonline.com/"),
        ("EntraId:TenantId", "00000000-0000-0000-0000-000000000001"),
        ("EntraId:ClientId", "00000000-0000-0000-0000-000000000002"),
        ("EntraId:Audience", "api://discover-costa-rica-tests"),
        ("Azure:TenantId", "00000000-0000-0000-0000-000000000001"),
        ("Azure:ClientId", "00000000-0000-0000-0000-000000000003"),
        ("Azure:ClientSecret", "not-a-secret"),
        ("Azure:Scope", "api://discover-costa-rica-tests/.default"),
    ];
}
