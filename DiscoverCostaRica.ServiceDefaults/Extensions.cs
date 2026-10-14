using System.Text.Json.Serialization;
using DiscoverCostaRica.ServiceDefaults.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Aspire service defaults shared by every service: OpenTelemetry, JSON, health checks, service
/// discovery, HTTP resilience, error handling and API docs. Service-specific wiring lives in each Program.cs.
/// </summary>
public static class Extensions
{
    /// <summary>Readiness: every registered check (SQL, Redis, ...) must pass.</summary>
    public const string HealthEndpointPath = "/health";

    /// <summary>Liveness: only checks tagged <c>live</c>, i.e. the process answers.</summary>
    public const string AlivenessEndpointPath = "/alive";

    public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.ConfigureOpenTelemetry();
        builder.AddJsonOptions();
        builder.AddDefaultHealthChecks();
        builder.AddClientCredentialsTokenProvider();
        builder.AddRedisClient(connectionName: "redis");

        builder.Services.AddServiceDiscovery();
        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Retry, circuit breaker and timeouts on every outbound call (Constitution VIII).
            http.AddStandardResilienceHandler();
            http.AddServiceDiscovery();
        });

        builder.AddGlobalException();

        return builder;
    }

    public static IHostApplicationBuilder AddGlobalException(this IHostApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        return builder;
    }

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

    public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation())
            .WithTracing(tracing => tracing
                .AddSource(builder.Environment.ApplicationName)
                .AddAspNetCoreInstrumentation(options =>
                    // Probes would drown the traces.
                    options.Filter = context =>
                        !context.Request.Path.StartsWithSegments(HealthEndpointPath)
                        && !context.Request.Path.StartsWithSegments(AlivenessEndpointPath))
                .AddHttpClientInstrumentation());

        if (!string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
            builder.Services.AddOpenTelemetry().UseOtlpExporter();

        return builder;
    }

    public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    /// <summary>
    /// Health probes for Azure Container Apps, in every environment (PLT-D6). Anonymous by design;
    /// the default writer only returns "Healthy", "Degraded" or "Unhealthy", never details.
    /// </summary>
    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        app.MapHealthChecks(HealthEndpointPath).AllowAnonymous();
        app.MapHealthChecks(AlivenessEndpointPath, new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") }).AllowAnonymous();

        return app;
    }

    private static IHostApplicationBuilder AddJsonOptions(this IHostApplicationBuilder builder)
    {
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });
        return builder;
    }
}
