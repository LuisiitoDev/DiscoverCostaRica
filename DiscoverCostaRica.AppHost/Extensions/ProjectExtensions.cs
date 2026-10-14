namespace DiscoverCostaRica.AppHost.Extensions;

public static class ResourceParameterBuilder
{
    private static IDictionary<string, IResourceBuilder<ParameterResource>>? _parameters;

    public static IResourceBuilder<ProjectResource> SetParameterForProject(IDistributedApplicationBuilder builder, IResourceBuilder<ProjectResource> project)
    {
        if (_parameters is not null) return SetParameters(project);

        var tenant = builder.AddParameter("azuretenant");
        _parameters = new Dictionary<string, IResourceBuilder<ParameterResource>>()
        {
            ["EntraId__Audience"] = builder.AddParameter("azureaudience"),
            ["EntraId__Instance"] = builder.AddParameter("azureinstance"),
            ["EntraId__ClientId"] = builder.AddParameter("azureapplication"),
            ["EntraId__TenantId"] = tenant,
            ["Azure__TenantId"] = tenant,
            ["Azure__ClientId"] = builder.AddParameter("azureclient"),
            ["Azure__ClientSecret"] = builder.AddParameter("azuresecret", secret: true),
            ["Azure__Scope"] = builder.AddParameter("azurescope"),
        };

        return SetParameters(project);
    }

    private static IResourceBuilder<ProjectResource> SetParameters(IResourceBuilder<ProjectResource> resource)
    {
        foreach (var parameter in _parameters!)
            resource.WithEnvironment(parameter.Key, parameter.Value);
        return resource;
    }
}

public static class ProjectExtensions
{
    /// <summary>A catalog service with its database, cache and Entra settings; <paramref name="dependsOn"/> for services it calls.</summary>
    public static IResourceBuilder<ProjectResource> CreateProject<TProject>(
        this IDistributedApplicationBuilder builder,
        string name,
        IResourceBuilder<IResourceWithConnectionString> azureSql,
        IResourceBuilder<IResourceWithConnectionString> redis,
        params IResourceBuilder<ProjectResource>[] dependsOn) where TProject : IProjectMetadata, new()
    {
        var project = builder.AddProject<TProject>(name)
            .WithReference(azureSql)
            .WaitFor(azureSql)
            .WithReference(redis)
            .WaitFor(redis)
            .WithHttpHealthCheck("/health");

        foreach (var dependency in dependsOn)
            project.WithReference(dependency).WaitFor(dependency);

        return ResourceParameterBuilder.SetParameterForProject(builder, project);
    }
}
