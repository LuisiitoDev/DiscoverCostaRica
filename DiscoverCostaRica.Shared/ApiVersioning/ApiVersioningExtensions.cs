using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace DiscoverCostaRica.Shared.ApiVersioning;

public static class ApiVersioningExtensions
{
    public static ApiVersionSet CreateGlobalVersionSet(this IEndpointRouteBuilder app)
    {
        return app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1.0))
            .HasApiVersion(new ApiVersion(1.0))
            .ReportApiVersions()
            .Build();
    }
}
