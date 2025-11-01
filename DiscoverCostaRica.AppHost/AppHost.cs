var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql").WithLifetime(ContainerLifetime.Persistent);
var db = sql.AddDatabase("DiscoverCostaRica");

builder.AddProject<Projects.DiscoverCostaRica_Beaches_Api>("discovercostarica-beaches-api");

builder.AddProject<Projects.DiscoverCostaRica_Culture_Api>("discovercostarica-cultureserice-api");

builder.AddProject<Projects.DiscoverCostaRica_Geo_Api>("discovercostarica-geoservice-api")
       .WithReference(db)
       .WaitFor(db);

builder.AddProject<Projects.DiscoverCostaRica_VolcanoService_Api>("discovercostarica-volcanoservice-api");

builder.Build().Run();
