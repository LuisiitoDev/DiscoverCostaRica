using Asp.Versioning;
using Asp.Versioning.Builder;
using DiscoverCostaRica.Shared.Routes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace DiscoverCostaRica.Shared.ApiVersioning;

public static class ApiVersioningExtensions
{
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
        return app.MapGroup($"/{RoutesConstants.ApiPrefix}/{RoutesConstants.ApiVersionParameter}{{version:apiVersion}}/{group}")
                  .WithApiVersionSet(versionSet)
                  .MapToApiVersion(version);
    }
}
