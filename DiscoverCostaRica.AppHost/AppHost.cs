var builder = DistributedApplication.CreateBuilder(args);

var existingSqlServerName = builder.AddParameter("existingSqlServerName");
var existingSqlServerResourceGroup = builder.AddParameter("existingSqlServerResourceGroup");

var azureSql = builder.AddAzureSqlServer("sqlserver")
                      .AsExisting(existingSqlServerName, existingSqlServerResourceGroup)
                      .AddDatabase("discovercostarica");

var mongoDb = builder.AddConnectionString("mongodb");

var azureFunction = builder.AddAzureFunctionsProject<Projects.DiscoverCostaRica_Functions>("discovercostarica-functions")
    .WithExternalHttpEndpoints()
    .WithReference(mongoDb)
    .WaitFor(mongoDb)
    .WithDaprSidecar();

var beaches = builder.AddProject<Projects.DiscoverCostaRica_Beaches_Api>("discovercostarica-beaches-api")
       .WithReference(azureSql)
       .WaitFor(azureSql)
       .WithReference(azureFunction)
       .WaitFor(azureFunction)
       .WithDaprSidecar();

builder.AddProject<Projects.DiscoverCostaRica_Culture_Api>("discovercostarica-cultureserice-api")
       .WithReference(azureSql)
       .WaitFor(azureSql)
       .WithReference(azureFunction)
       .WaitFor(azureFunction)
       .WithDaprSidecar();

builder.AddProject<Projects.DiscoverCostaRica_Geo_Api>("discovercostarica-geoservice-api")
       .WithReference(azureSql)
       .WaitFor(azureSql)
       .WithReference(azureFunction)
       .WaitFor(azureFunction)
       .WithDaprSidecar();

builder.AddProject<Projects.DiscoverCostaRica_Volcano_Api>("discovercostarica-volcanoservice-api")
       .WithReference(azureSql)
       .WaitFor(azureSql)
       .WithReference(azureFunction)
       .WaitFor(azureFunction)
       .WithDaprSidecar();

builder.AddYarp("gateway")
       .WithConfiguration(yarp =>
       {
           yarp.AddRoute(beaches);
       });

//var gateway = builder.AddYarp("gateway");

builder.Build().Run();
