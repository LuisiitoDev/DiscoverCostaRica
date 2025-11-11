using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscoverCostaRica.Tests.Infraestructure;

/// <summary>
/// Extensions for configuring test authentication in integration tests
/// </summary>
public static class TestAuthenticationExtensions
{
    /// <summary>
    /// Startup filter that replaces Entra ID authentication with test authentication
    /// </summary>
    private class TestAuthenticationStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                // Configure test authentication to replace Entra ID
                var services = app.ApplicationServices;
                
                // The authentication middleware will use TestAuthenticationHandler
                // which is configured through the authentication builder
                next(app);
            };
        }
    }

    /// <summary>
    /// Configures test authentication for a service in testing environment
    /// This should be called after AddEntraIdAuthentication to replace it with test auth
    /// </summary>
    public static IHostApplicationBuilder ConfigureTestAuthentication(this IHostApplicationBuilder builder)
    {
        // Only apply in Testing environment
        if (builder.Environment.EnvironmentName != "Testing")
        {
            return builder;
        }

        // Check if test authentication should be used
        var useTestAuth = builder.Configuration["Testing:UseTestAuthentication"];
        if (useTestAuth != "true")
        {
            return builder;
        }

        // Clear existing authentication and add test authentication
        // This replaces the Entra ID authentication with test authentication
        builder.Services.AddAuthentication(TestAuthenticationHandler.AuthenticationScheme)
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                TestAuthenticationHandler.AuthenticationScheme,
                options => { });

        // Add startup filter to ensure test authentication is applied
        builder.Services.AddSingleton<IStartupFilter, TestAuthenticationStartupFilter>();

        return builder;
    }
}
