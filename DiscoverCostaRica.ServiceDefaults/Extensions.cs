using Asp.Versioning;
using DiscoverCostaRica.ServiceDefaults.Middleware;
using DiscoverCostaRica.Shared.logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;

namespace Microsoft.Extensions.Hosting;

// Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
public static class Extensions
{
    private const string HealthEndpointPath = "/health";
    private const string AlivenessEndpointPath = "/alive";

    public static WebApplication AddScalar(this WebApplication app)
    {
        app.MapScalarApiReference("/docs", options =>
        {
            options.Title = app.Environment.ApplicationName;
            options.Theme = ScalarTheme.Kepler;
            options.DarkMode = true;
        });
        return app;
    }

    public static IHostApplicationBuilder AddVersioning(this IHostApplicationBuilder builder)
    {
        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });
        return builder;
    }

    public static IHostApplicationBuilder AddFunctionLogger(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ILoggerProvider, DiscoverCostaRicaLoggerProvider>();

        return builder;
    }

    public static IHostApplicationBuilder AddGlobalExeption(this IHostApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        return builder;
    }

    public static IHostApplicationBuilder AddMappingProfile<TProfile>(this IHostApplicationBuilder builder)
        where TProfile : AutoMapper.Profile
    {
        builder.Services.AddAutoMapper(cfg => cfg.AddProfile(typeof(TProfile)));
        return builder;
    }

    public static IHostApplicationBuilder AddDiscoverCostaRicaContext<TInterfaceContext, Context>(this IHostApplicationBuilder builder, string connectionStringKey = "DiscoverCostaRica")
    where TInterfaceContext : class
    where Context : DbContext, TInterfaceContext


    {
        builder.AddSqlServerDbContext<Context>(connectionName: "DiscoverCostaRica");
        builder.Services.AddScoped<TInterfaceContext, Context>();

        return builder;
    }

    public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.ConfigureOpenTelemetry();

        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler();

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        // Uncomment the following to restrict the allowed schemes for service discovery.
        // builder.Services.Configure<ServiceDiscoveryOptions>(options =>
        // {
        //     options.AllowedSchemes = ["https"];
        // });

        return builder;
    }

    public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing.AddSource(builder.Environment.ApplicationName)
                    .AddAspNetCoreInstrumentation(tracing =>
                        // Exclude health check requests from tracing
                        tracing.Filter = context =>
                            !context.Request.Path.StartsWithSegments(HealthEndpointPath)
                            && !context.Request.Path.StartsWithSegments(AlivenessEndpointPath)
                    )
                    // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                    //.AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    private static TBuilder AddOpenTelemetryExporters<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
        //if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
        //{
        //    builder.Services.AddOpenTelemetry()
        //       .UseAzureMonitor();
        //}

        return builder;
    }

    public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.MapHealthChecks(HealthEndpointPath);

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks(AlivenessEndpointPath, new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        return app;
    }

    public static IHostApplicationBuilder AddEntraIdAuthentication(this IHostApplicationBuilder builder)
    {
        // Read EntraId configuration section
        var entraIdSection = builder.Configuration.GetSection(DiscoverCostaRica.Shared.Authentication.EntraIdOptions.SectionName);
        var entraIdOptions = entraIdSection.Get<DiscoverCostaRica.Shared.Authentication.EntraIdOptions>();

        if (entraIdOptions == null || string.IsNullOrWhiteSpace(entraIdOptions.TenantId))
        {
            // EntraId not configured - skip authentication setup
            // Note: Logging will occur when the application starts and the logger is available
            return builder;
        }

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<DiscoverCostaRica.Shared.Authentication.ICurrentUserService, 
                                   DiscoverCostaRica.Shared.Authentication.CurrentUserService>();

        // Configure JWT Bearer authentication with Microsoft.Identity.Web
        builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(jwtOptions =>
            {
                jwtOptions.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler>>();
                        logger.LogError(context.Exception, "Authentication failed: {Message}", context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler>>();
                        var userId = context.Principal?.FindFirst(DiscoverCostaRica.Shared.Authentication.AuthConstants.ClaimTypes.ObjectId)?.Value
                                    ?? context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                        logger.LogInformation("Token validated successfully for user: {UserId}", userId ?? "Unknown");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler>>();
                        logger.LogWarning("Authentication challenge issued: {Error}, {ErrorDescription}", 
                            context.Error, context.ErrorDescription);
                        return Task.CompletedTask;
                    }
                };
                
                // Set audience if configured
                if (!string.IsNullOrWhiteSpace(entraIdOptions.Audience))
                {
                    jwtOptions.TokenValidationParameters.ValidAudiences = new[] { entraIdOptions.Audience };
                }
            }, identityOptions =>
            {
                identityOptions.Instance = entraIdOptions.Instance;
                identityOptions.TenantId = entraIdOptions.TenantId;
                identityOptions.ClientId = entraIdOptions.ClientId;
            });

        // Configure authorization policies based on scopes
        // JWT tokens contain space-separated scopes in a single claim value
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(DiscoverCostaRica.Shared.Authentication.AuthConstants.Policies.BeachesRead, policy =>
                policy.RequireAssertion(context =>
                    context.User.Claims
                        .Where(c => c.Type == DiscoverCostaRica.Shared.Authentication.AuthConstants.ClaimTypes.Scope || 
                                   c.Type == DiscoverCostaRica.Shared.Authentication.AuthConstants.ClaimTypes.ScopeShort)
                        .Any(c => c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Contains(DiscoverCostaRica.Shared.Authentication.AuthConstants.Scopes.BeachesRead))))
            .AddPolicy(DiscoverCostaRica.Shared.Authentication.AuthConstants.Policies.BeachesWrite, policy =>
                policy.RequireAssertion(context =>
                    context.User.Claims
                        .Where(c => c.Type == DiscoverCostaRica.Shared.Authentication.AuthConstants.ClaimTypes.Scope || 
                                   c.Type == DiscoverCostaRica.Shared.Authentication.AuthConstants.ClaimTypes.ScopeShort)
                        .Any(c => c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Contains(DiscoverCostaRica.Shared.Authentication.AuthConstants.Scopes.BeachesWrite))))
            .AddPolicy(DiscoverCostaRica.Shared.Authentication.AuthConstants.Policies.VolcanoRead, policy =>
                policy.RequireAssertion(context =>
                    context.User.Claims
                        .Where(c => c.Type == DiscoverCostaRica.Shared.Authentication.AuthConstants.ClaimTypes.Scope || 
                                   c.Type == DiscoverCostaRica.Shared.Authentication.AuthConstants.ClaimTypes.ScopeShort)
                        .Any(c => c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Contains(DiscoverCostaRica.Shared.Authentication.AuthConstants.Scopes.VolcanoRead))))
            .AddPolicy(DiscoverCostaRica.Shared.Authentication.AuthConstants.Policies.VolcanoWrite, policy =>
                policy.RequireAssertion(context =>
                    context.User.Claims
                        .Where(c => c.Type == DiscoverCostaRica.Shared.Authentication.AuthConstants.ClaimTypes.Scope || 
                                   c.Type == DiscoverCostaRica.Shared.Authentication.AuthConstants.ClaimTypes.ScopeShort)
                        .Any(c => c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Contains(DiscoverCostaRica.Shared.Authentication.AuthConstants.Scopes.VolcanoWrite))))
            .AddPolicy(DiscoverCostaRica.Shared.Authentication.AuthConstants.Policies.CultureRead, policy =>
                policy.RequireAssertion(context =>
                    context.User.Claims
                        .Where(c => c.Type == DiscoverCostaRica.Shared.Authentication.AuthConstants.ClaimTypes.Scope || 
                                   c.Type == DiscoverCostaRica.Shared.Authentication.AuthConstants.ClaimTypes.ScopeShort)
                        .Any(c => c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Contains(DiscoverCostaRica.Shared.Authentication.AuthConstants.Scopes.CultureRead))))
            .AddPolicy(DiscoverCostaRica.Shared.Authentication.AuthConstants.Policies.CultureWrite, policy =>
                policy.RequireAssertion(context =>
                    context.User.Claims
                        .Where(c => c.Type == DiscoverCostaRica.Shared.Authentication.AuthConstants.ClaimTypes.Scope || 
                                   c.Type == DiscoverCostaRica.Shared.Authentication.AuthConstants.ClaimTypes.ScopeShort)
                        .Any(c => c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Contains(DiscoverCostaRica.Shared.Authentication.AuthConstants.Scopes.CultureWrite))))
            .AddPolicy(DiscoverCostaRica.Shared.Authentication.AuthConstants.Policies.GeoRead, policy =>
                policy.RequireAssertion(context =>
                    context.User.Claims
                        .Where(c => c.Type == DiscoverCostaRica.Shared.Authentication.AuthConstants.ClaimTypes.Scope || 
                                   c.Type == DiscoverCostaRica.Shared.Authentication.AuthConstants.ClaimTypes.ScopeShort)
                        .Any(c => c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Contains(DiscoverCostaRica.Shared.Authentication.AuthConstants.Scopes.GeoRead))))
            .AddPolicy(DiscoverCostaRica.Shared.Authentication.AuthConstants.Policies.GeoWrite, policy =>
                policy.RequireAssertion(context =>
                    context.User.Claims
                        .Where(c => c.Type == DiscoverCostaRica.Shared.Authentication.AuthConstants.ClaimTypes.Scope || 
                                   c.Type == DiscoverCostaRica.Shared.Authentication.AuthConstants.ClaimTypes.ScopeShort)
                        .Any(c => c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Contains(DiscoverCostaRica.Shared.Authentication.AuthConstants.Scopes.GeoWrite))));

        return builder;
    }
}
