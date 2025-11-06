var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("DiscoverCostaRicaServer").WithLifetime(ContainerLifetime.Persistent);
var db = sql.AddDatabase("DiscoverCostaRica");

var mongo = builder.AddMongoDB("DiscoverCostaRicaMongo").WithLifetime(ContainerLifetime.Persistent);
var mongoDb = mongo.AddDatabase("logger");

var azureFunction = builder.AddAzureFunctionsProject<Projects.DiscoverCostaRica_Functions>("discovercostarica-functions")
    .WithExternalHttpEndpoints()
    .WithReference(mongoDb)
    .WaitFor(mongoDb)
    .WithDaprSidecar();

builder.AddProject<Projects.DiscoverCostaRica_Beaches_Api>("discovercostarica-beaches-api")
       .WithReference(db)
       .WaitFor(db)
       .WithReference(azureFunction)
       .WaitFor(azureFunction)
       .WithDaprSidecar();

builder.AddProject<Projects.DiscoverCostaRica_Culture_Api>("discovercostarica-cultureserice-api")
       .WithReference(db)
       .WaitFor(db)
       .WithReference(azureFunction)
       .WaitFor(azureFunction)
       .WithDaprSidecar();

builder.AddProject<Projects.DiscoverCostaRica_Geo_Api>("discovercostarica-geoservice-api")
       .WithReference(db)
       .WaitFor(db)
       .WithReference(azureFunction)
       .WaitFor(azureFunction)
       .WithDaprSidecar();

builder.AddProject<Projects.DiscoverCostaRica_Volcano_Api>("discovercostarica-volcanoservice-api")
       .WithReference(db)
       .WaitFor(db)
       .WithReference(azureFunction)
       .WaitFor(azureFunction)
       .WithDaprSidecar();




builder.Build().Run();
