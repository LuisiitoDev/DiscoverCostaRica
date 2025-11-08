using Aspire.Hosting.Yarp.Transforms;

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

var culture = builder.AddProject<Projects.DiscoverCostaRica_Culture_Api>("discovercostarica-cultureserice-api")
       .WithReference(azureSql)
       .WaitFor(azureSql)
       .WithReference(azureFunction)
       .WaitFor(azureFunction)
       .WithDaprSidecar();

var geo = builder.AddProject<Projects.DiscoverCostaRica_Geo_Api>("discovercostarica-geoservice-api")
       .WithReference(azureSql)
       .WaitFor(azureSql)
       .WithReference(azureFunction)
       .WaitFor(azureFunction)
       .WithDaprSidecar();

var volcano = builder.AddProject<Projects.DiscoverCostaRica_Volcano_Api>("discovercostarica-volcanoservice-api")
       .WithReference(azureSql)
       .WaitFor(azureSql)
       .WithReference(azureFunction)
       .WaitFor(azureFunction)
       .WithDaprSidecar();

// Configure YARP gateway with service-specific routes
// YARP automatically forwards all request headers including Authorization
builder.AddYarp("gateway")
       .WithExternalHttpEndpoints()
       .WithReference(beaches)
       .WithReference(culture)
       .WithReference(geo)
       .WithReference(volcano)
       .WithConfiguration(yarp =>
       {
           // Beaches service routes: /api/beaches/* -> /api/v1/*
           yarp.AddRoute("/api/beaches/{**catch-all}", beaches)
               .WithTransformPathRemovePrefix("/api/beaches")
               .WithTransformPathPrefix("/api/v1");

           // Culture service routes: /api/culture/* -> /api/v1/*
           yarp.AddRoute("/api/culture/{**catch-all}", culture)
               .WithTransformPathRemovePrefix("/api/culture")
               .WithTransformPathPrefix("/api/v1");

           // Geo service routes: /api/geo/* -> /api/v1/*
           yarp.AddRoute("/api/geo/{**catch-all}", geo)
               .WithTransformPathRemovePrefix("/api/geo")
               .WithTransformPathPrefix("/api/v1");

           // Volcano service routes: /api/volcano/* -> /api/v1/*
           yarp.AddRoute("/api/volcano/{**catch-all}", volcano)
               .WithTransformPathRemovePrefix("/api/volcano")
               .WithTransformPathPrefix("/api/v1");
       });

builder.Build().Run();
