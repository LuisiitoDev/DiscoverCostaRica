using Aspire.Hosting.Yarp.Transforms;
using DiscoverCostaRica.AppHost.Constants;
using DiscoverCostaRica.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

var existingSqlServerName = builder.AddParameter("existingSqlServerName", secret: true);
var existingSqlServerResourceGroup = builder.AddParameter("existingSqlServerResourceGroup", secret: true);

var azureSql = builder.AddAzureSqlServer("sqlserver")
                      .AsExisting(existingSqlServerName, existingSqlServerResourceGroup)
                      .AddDatabase("discovercostarica");

var mongoDb = builder.AddConnectionString("mongodb");

var azureFunction = builder.AddAzureFunctionsProject<Projects.DiscoverCostaRica_Functions>("discovercostarica-functions")
    .WithExternalHttpEndpoints()
    .WithReference(mongoDb)
    .WaitFor(mongoDb)
    .WithDaprSidecar();

var beaches = builder.CreateProject<Projects.DiscoverCostaRica_Beaches_Api>(Microservices.Beaches, azureSql, azureFunction);
var culture = builder.CreateProject<Projects.DiscoverCostaRica_Culture_Api>(Microservices.Culture, azureSql, azureFunction);
var geo = builder.CreateProject<Projects.DiscoverCostaRica_Geo_Api>(Microservices.Geo, azureSql, azureFunction);
var volcano = builder.CreateProject<Projects.DiscoverCostaRica_Volcano_Api>(Microservices.Volcano, azureSql, azureFunction);

builder.AddYarp(Microservices.Gateway)
       .WithDeveloperCertificateTrust(true)
       .WithHttpsEndpoint(port: 8081, targetPort: 8081)
       .WithHostPort(8081)
       .WithConfiguration(yarp =>
       {
           yarp.AddRoute("/provinces/{**catch-all}", geo)
               .WithTransformPathPrefix("/api/v1/geo");
       });

builder.Build().Run();


