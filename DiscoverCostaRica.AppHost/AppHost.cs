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


builder.AddYarp("gateway")
       .WithExternalHttpEndpoints()
       .WithReference(beaches)
       .WaitFor(beaches)
       .WithReference(culture)
       .WaitFor(culture)
       .WithReference(geo)
       .WaitFor(geo)
       .WithReference(volcano)
       .WaitFor(volcano)
       .WithConfiguration(yarp =>
       {
           yarp.AddRoute("/api/beaches/{**catch-all}", beaches)
               .WithTransformPathRemovePrefix("/api/beaches")
               .WithTransformPathPrefix("/api/v1");

           yarp.AddRoute("/api/culture/{**catch-all}", culture)
               .WithTransformPathRemovePrefix("/api/culture")
               .WithTransformPathPrefix("/api/v1");

           yarp.AddRoute("/api/geo/{**catch-all}", geo)
               .WithTransformPathRemovePrefix("/api/geo")
               .WithTransformPathPrefix("/api/v1");

           yarp.AddRoute("/api/volcano/{**catch-all}", volcano)
               .WithTransformPathRemovePrefix("/api/volcano")
               .WithTransformPathPrefix("/api/v1");
       });

builder.Build().Run();
