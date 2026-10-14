using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace DiscoverCostaRica.BuildingBlocks.Web.Versioning;

public static class ApiVersioningExtensions
{
    /// <summary>Every route is <c>/api/v{version}/{group}/...</c>.</summary>
    public const string ApiPrefix = "api";

    public static ApiVersionSet CreateGlobalVersionSet(this IEndpointRouteBuilder app)
    {
        return app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1.0))
            .HasApiVersion(new ApiVersion(2.0))
            .ReportApiVersions()
            .Build();
    }

    public static RouteGroupBuilder BuildEndpointGroup(this IEndpointRouteBuilder app, double version, string group)
    {
        var versionSet = app.CreateGlobalVersionSet();
        return app.MapGroup($"/{ApiPrefix}/v{{version:apiVersion}}/{group}")
                  .WithApiVersionSet(versionSet)
                  .MapToApiVersion(version);
    }
}
