var builder = DistributedApplication.CreateBuilder(args);

var rabbitMq = builder.AddRabbitMQ("LoggerMessaging");

var sql = builder.AddSqlServer("sql").WithLifetime(ContainerLifetime.Persistent);
var db = sql.AddDatabase("DiscoverCostaRica");

builder.AddProject<Projects.DiscoverCostaRica_Beaches_Api>("discovercostarica-beaches-api")
       .WithReference(db)
       .WaitFor(db)
       .WithReference(rabbitMq);

builder.AddProject<Projects.DiscoverCostaRica_Culture_Api>("discovercostarica-cultureserice-api")
       .WithReference(db)
       .WaitFor(db)
       .WithReference(rabbitMq);

builder.AddProject<Projects.DiscoverCostaRica_Geo_Api>("discovercostarica-geoservice-api")
       .WithReference(db)
       .WaitFor(db)
       .WithReference(rabbitMq);

builder.AddProject<Projects.DiscoverCostaRica_VolcanoService_Api>("discovercostarica-volcanoservice-api")
       .WithReference(db)
       .WaitFor(db)
       .WithReference(rabbitMq);

builder.AddAzureFunctionsProject<Projects.DiscoverCostaRica_Function_LogConsumer>("discovercostarica-function-logconsumer");

builder.Build().Run();
