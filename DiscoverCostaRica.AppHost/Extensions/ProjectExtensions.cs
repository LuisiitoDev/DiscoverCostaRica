using CommunityToolkit.Aspire.Hosting.Dapr;

namespace DiscoverCostaRica.AppHost.Extensions;

public static class ProjectExtensions
{
    public static IResourceBuilder<ProjectResource> CreateProject<TProject>(
        this IDistributedApplicationBuilder builder,
        string name,
        IResourceBuilder<IResourceWithConnectionString> azureSql,
        IResourceBuilder<IDaprComponentResource> secretStore,
        IResourceBuilder<IDaprComponentResource> binding) where TProject : IProjectMetadata, new()
    {
        return builder.AddProject<TProject>(name)
               .WithReference(azureSql)
               .WaitFor(azureSql)
               .WithDaprSidecar(options =>
               {
                   options.WithReference(secretStore);
                   options.WithReference(binding);
               });
    }

    public static IResourceBuilder<IDaprComponentResource> CreateDaprComponent(this IDistributedApplicationBuilder builder, string name, string type, string route)
    {
        return builder.AddDaprComponent(name, type, new DaprComponentOptions
        {
            LocalPath = route
        });
    }
}
