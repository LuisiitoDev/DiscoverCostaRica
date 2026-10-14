using Aspire.Hosting.Yarp.Transforms;
using DiscoverCostaRica.AppHost.Constants;
using DiscoverCostaRica.AppHost.Extensions;
using DiscoverCostaRica.AppHost.Gateway;

var builder = DistributedApplication.CreateBuilder(args);

var existingSqlServerName = builder.AddParameter("existingSqlServerName", secret: true);
var existingSqlServerResourceGroup = builder.AddParameter("existingSqlServerResourceGroup", secret: true);

var azureSql = builder.AddAzureSqlServer("sqlserver")
                      .AsExisting(existingSqlServerName, existingSqlServerResourceGroup)
                      .AddDatabase("discovercostarica");

var cache = builder.AddConnectionString("redis");

var geo = builder.CreateProject<Projects.DiscoverCostaRica_Geo_Api>(Microservices.Geo, azureSql, cache);
var services = new Dictionary<string, IResourceBuilder<ProjectResource>>
{
    [Microservices.Geo] = geo,
    [Microservices.Beaches] = builder.CreateProject<Projects.DiscoverCostaRica_Beaches_Api>(Microservices.Beaches, azureSql, cache),
    [Microservices.Culture] = builder.CreateProject<Projects.DiscoverCostaRica_Culture_Api>(Microservices.Culture, azureSql, cache),
    // Volcano resolves location names through Geo's batch contract.
    [Microservices.Volcano] = builder.CreateProject<Projects.DiscoverCostaRica_Volcano_Api>(Microservices.Volcano, azureSql, cache, geo),
};

builder.AddYarp(Microservices.Gateway)
    .WithDeveloperCertificateTrust(true)
    .WithConfiguration(yarp =>
    {
        foreach (var route in GatewayRoutes.All)
            yarp.AddRoute(route.Path, services[route.Service]).WithTransformPathRouteValues(route.Upstream);
    });

await builder.Build().RunAsync();
