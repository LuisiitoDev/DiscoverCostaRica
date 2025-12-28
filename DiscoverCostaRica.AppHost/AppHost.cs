using Aspire.Hosting.Yarp.Transforms;
using DiscoverCostaRica.AppHost.Constants;
using DiscoverCostaRica.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

var existingSqlServerName = builder.AddParameter("existingSqlServerName", secret: true);
var existingSqlServerResourceGroup = builder.AddParameter("existingSqlServerResourceGroup", secret: true);

var azureSql = builder.AddAzureSqlServer("sqlserver")
                      .AsExisting(existingSqlServerName, existingSqlServerResourceGroup)
                      .AddDatabase("discovercostarica");

var cache = builder.AddConnectionString("redis");
var mongodb = builder.AddConnectionString("mongodb");

var geo = builder.CreateProject<Projects.DiscoverCostaRica_Geo_Api>(Microservices.Geo, azureSql, mongodb, cache);
var beaches = builder.CreateProject<Projects.DiscoverCostaRica_Beaches_Api>(Microservices.Beaches, azureSql, mongodb, cache, geo);
var culture = builder.CreateProject<Projects.DiscoverCostaRica_Culture_Api>(Microservices.Culture, azureSql, mongodb, cache, geo);
var volcano = builder.CreateProject<Projects.DiscoverCostaRica_Volcano_Api>(Microservices.Volcano, azureSql, mongodb, cache, geo);

builder.AddYarp(Microservices.Gateway)
    .WithDeveloperCertificateTrust(true)
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

await builder.Build().RunAsync();


