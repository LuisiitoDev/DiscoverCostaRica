using CommunityToolkit.Aspire.Hosting.Dapr;
using System.Collections.Immutable;

namespace DiscoverCostaRica.AppHost.Extensions;

public static class ProjectExtensions
{
    public static IResourceBuilder<ProjectResource> CreateProject<TProject>(
        this IDistributedApplicationBuilder builder,
        string name,
        IResourceBuilder<IResourceWithConnectionString> azureSql) where TProject : IProjectMetadata, new()
    {
        return builder.AddProject<TProject>(name)
               .WithReference(azureSql)
               .WaitFor(azureSql)
               .WithDaprSidecar(options =>
               {
                   options.WithOptions(new DaprSidecarOptions()
                   {
                       Config = "../k8s/config.yaml",
                       ResourcesPaths = ImmutableHashSet.Create("../k8s")
                   });
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
