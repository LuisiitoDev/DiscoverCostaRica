var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql").WithLifetime(ContainerLifetime.Persistent);
var db = sql.AddDatabase("DiscoverCostaRica");

builder.AddProject<Projects.DiscoverCostaRica_Api>("discovercostarica-api");

builder.AddProject<Projects.DiscoverCostaRica_Beaches_Api>("discovercostarica-beaches-api");

builder.AddProject<Projects.DiscoverCostaRica_CultureSerice_Api>("discovercostarica-cultureserice-api");

builder.AddProject<Projects.DiscoverCostaRica_GeoService_Api>("discovercostarica-geoservice-api")
       .WithReference(db)
       .WaitFor(db);

builder.AddProject<Projects.DiscoverCostaRica_VolcanoService_Api>("discovercostarica-volcanoservice-api");

builder.Build().Run();
