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


builder.AddProject<Projects.DiscoverCostaRica_Gateway>(Microservices.Gateway)
       .WithReference(geo).WaitFor(geo)
       .WithReference(beaches).WaitFor(beaches)
       .WithReference(culture).WaitFor(culture)
       .WithReference(volcano).WaitFor(volcano)
       .WithExternalHttpEndpoints();

builder.Build().Run();


