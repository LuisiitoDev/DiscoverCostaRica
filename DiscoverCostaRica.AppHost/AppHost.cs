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

var logggerFunction = builder.CreateFunctionProject<Projects.DiscoverCostaRica_Functions>(Microservices.Function, mongoDb);
var beaches = builder.CreateProject<Projects.DiscoverCostaRica_Beaches_Api>(Microservices.Beaches, azureSql, logggerFunction);
var culture = builder.CreateProject<Projects.DiscoverCostaRica_Culture_Api>(Microservices.Culture, azureSql, logggerFunction);
var geo = builder.CreateProject<Projects.DiscoverCostaRica_Geo_Api>(Microservices.Geo, azureSql, logggerFunction);
var volcano = builder.CreateProject<Projects.DiscoverCostaRica_Volcano_Api>(Microservices.Volcano, azureSql, logggerFunction);

builder.AddYarp(Microservices.Gateway)
       .WithDeveloperCertificateTrust(true)
       .WithHttpsEndpoint(port: 8081, targetPort: 8081)
       .WithHostPort(8081)
       .WithConfiguration(yarp =>
       {
           yarp.AddRoute("/provinces/{**catch-all}", geo).WithTransformPathPrefix("/api/v1/geo");
           yarp.AddRoute("/canton/{**catch-all}", geo).WithTransformPathPrefix("/api/v1/geo");
           yarp.AddRoute("/districts/{**catch-all}", geo).WithTransformPathPrefix("/api/v1/geo");

           yarp.AddRoute("/beaches/{**catch-all}", beaches).WithTransformPathPrefix("/api/v1/beaches");

           yarp.AddRoute("/tradition/{**catch-all}", culture).WithTransformPathPrefix("/api/v1/traditions");
           yarp.AddRoute("/dish/{**catch-all}", culture).WithTransformPathPrefix("/api/v1/traditions");

           yarp.AddRoute("/volcano/{**catch-all}", volcano).WithTransformPathPrefix("/api/v1/volcanoes");
           yarp.AddRoute("/province/{**catch-all}", volcano).WithTransformPathPrefix("/api/v1/volcanoes");
           yarp.AddRoute("/{**catch-all}", volcano).WithTransformPathPrefix("/api/v1/volcanoes");
       });

builder.Build().Run();


