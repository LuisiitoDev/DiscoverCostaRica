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
       .WithConfiguration(yarp =>
       {
           yarp.AddRoute("/api/{**catch-all}", beaches)
               .WithTransformPathRemovePrefix("/api")
               .WithTransformPathPrefix("/v1");
           yarp.AddRoute("/api/{**catch-all}", culture)
               .WithTransformPathRemovePrefix("/api")
               .WithTransformPathPrefix("/v1");
           yarp.AddRoute("/api/{**catch-all}", geo)
               .WithTransformPathRemovePrefix("/api")
               .WithTransformPathPrefix("/v1");
           yarp.AddRoute("/api/{**catch-all}", volcano)
               .WithTransformPathRemovePrefix("/api")
               .WithTransformPathPrefix("/v1");
       });

//var gateway = builder.AddYarp("gateway");

builder.Build().Run();
