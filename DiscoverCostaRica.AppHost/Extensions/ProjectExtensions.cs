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
            .WaitFor(azureSql);
    }

    public static IResourceBuilder<ProjectResource> CreateProject<TProject>(
        this IDistributedApplicationBuilder builder,
        string name,
        IResourceBuilder<IResourceWithConnectionString> azureSql,
        IResourceBuilder<ProjectResource> geo) where TProject : IProjectMetadata, new()
    {
        return builder.AddProject<TProject>(name)
            .WithReference(azureSql)
            .WaitFor(azureSql)
            .WithReference(geo)
            .WaitFor(geo);
    }
}
