using Asp.Versioning;
using Asp.Versioning.Builder;

namespace DiscoverCostaRica.Api.Configuration;

public class VersioningConfiguration
{
    public static ApiVersionSet GetVersion(WebApplication app) => app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .HasApiVersion(new ApiVersion(2, 0))
            .ReportApiVersions()
            .Build();
}
