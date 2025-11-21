using Aspire.Hosting.Azure;

namespace DiscoverCostaRica.AppHost.Extensions;

public static class ProjectExtensions
{
    public static IResourceBuilder<ProjectResource> CreateProject<TProject>(
        this IDistributedApplicationBuilder builder, 
        string name,
        IResourceBuilder<IResourceWithConnectionString> azureSql,
        IResourceBuilder<IResourceWithServiceDiscovery> azureFunction) where TProject : IProjectMetadata, new()
    {
        return builder.AddProject<TProject>(name)
               .WithReference(azureSql)
               .WaitFor(azureSql)
               .WithReference(azureFunction)
               .WaitFor(azureFunction)
               .WithDaprSidecar();
    }
}
