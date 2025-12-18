using Aspire.Hosting.ApplicationModel;

namespace DiscoverCostaRica.AppHost.Extensions;

public static class ProjectExtensions
{
    public static IResourceBuilder<ProjectResource> CreateProject<TProject>(this IDistributedApplicationBuilder builder,
        string name,
        IResourceBuilder<IResourceWithConnectionString> azureSql,
        IResourceBuilder<IResourceWithConnectionString> mongodb) where TProject : IProjectMetadata, new()
    {
        return BuildWithEntraParameters(
            builder,
            builder.AddProject<TProject>(name)
                .WithReference(azureSql)
                .WaitFor(azureSql)
                .WithReference(mongodb)
                .WaitFor(mongodb)
        );
    }

    public static IResourceBuilder<ProjectResource> CreateProject<TProject>(
        this IDistributedApplicationBuilder builder,
        string name,
        IResourceBuilder<IResourceWithConnectionString> azureSql,
        IResourceBuilder<IResourceWithConnectionString> mongodb,
        IResourceBuilder<ProjectResource> geo) where TProject : IProjectMetadata, new()
    {
        return BuildWithEntraParameters(
            builder,
            builder.AddProject<TProject>(name)
                .WithReference(azureSql)
                .WaitFor(azureSql)
                .WithReference(mongodb)
                .WaitFor(mongodb)
                .WithReference(geo)
                .WaitFor(geo)
        );
    }

    private static IResourceBuilder<ProjectResource> BuildWithEntraParameters(IDistributedApplicationBuilder builder, IResourceBuilder<ProjectResource> project)
    {
        var applicationAudience = builder.AddParameter("applicationAudience");
        var applicationInstance = builder.AddParameter("applicationInstance");
        var applicationId = builder.AddParameter("applicationId");
        var tenantId = builder.AddParameter("tenantId");
        var clientId = builder.AddParameter("clientId");
        var clientSecret = builder.AddParameter("clientSecret", secret: true);
        var scope = builder.AddParameter("scope");

        return project
            .WithEnvironment("Azure__TenantId", tenantId)
            .WithEnvironment("Azure__ClientId", clientId)
            .WithEnvironment("Azure__Scope", scope)
            .WithEnvironment("Azure__ClientSecret", clientSecret)
            .WithEnvironment("EntraId__Instance", applicationInstance)
            .WithEnvironment("EntraId__TenantId", tenantId)
            .WithEnvironment("EntraId__ClientId", applicationId)
            .WithEnvironment("EntraId__Audience", applicationAudience);
    }
}
