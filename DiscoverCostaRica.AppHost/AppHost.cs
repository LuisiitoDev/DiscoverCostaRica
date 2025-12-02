using Aspire.Hosting.Yarp.Transforms;
using DiscoverCostaRica.AppHost.Constants;
using DiscoverCostaRica.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

var existingSqlServerName = builder.AddParameter("existingSqlServerName", secret: true);
var existingSqlServerResourceGroup = builder.AddParameter("existingSqlServerResourceGroup", secret: true);

var azureSql = builder.AddAzureSqlServer("sqlserver")
                      .AsExisting(existingSqlServerName, existingSqlServerResourceGroup)
                      .AddDatabase("discovercostarica");
var mongodb = builder.AddConnectionString("mongodb");
var applicationAudience = builder.AddParameter("applicationAudience");
var applicationInstance = builder.AddParameter("applicationInstance");
var applicationId = builder.AddParameter("applicationId");
var tenantId = builder.AddParameter("tenantId");
var clientId = builder.AddParameter("clientId");
var clientSecret = builder.AddParameter("clientSecret", secret: true);
var scope = builder.AddParameter("scope");

var geo = builder.CreateProject<Projects.DiscoverCostaRica_Geo_Api>(Microservices.Geo, azureSql, mongodb)
          .WithEnvironment("Azure__TenantId", tenantId)
          .WithEnvironment("Azure__ClientId", clientId)
          .WithEnvironment("Azure__Scope", scope)
          .WithEnvironment("Azure__ClientSecret", clientSecret)
          .WithEnvironment("EntraId__Instance", applicationInstance)
          .WithEnvironment("EntraId__TenantId", tenantId)
          .WithEnvironment("EntraId__ClientId", applicationId)
          .WithEnvironment("EntraId__Audience", applicationAudience);

var beaches = builder.CreateProject<Projects.DiscoverCostaRica_Beaches_Api>(Microservices.Beaches, azureSql, mongodb, geo)
          .WithEnvironment("Azure__TenantId", tenantId)
          .WithEnvironment("Azure__ClientId", clientId)
          .WithEnvironment("Azure__Scope", scope)
          .WithEnvironment("Azure__ClientSecret", clientSecret)
          .WithEnvironment("EntraId__Instance", applicationInstance)
          .WithEnvironment("EntraId__TenantId", tenantId)
          .WithEnvironment("EntraId__ClientId", applicationId)
          .WithEnvironment("EntraId__Audience", applicationAudience);

var culture = builder.CreateProject<Projects.DiscoverCostaRica_Culture_Api>(Microservices.Culture, azureSql, mongodb, geo)
          .WithEnvironment("Azure__TenantId", tenantId)
          .WithEnvironment("Azure__ClientId", clientId)
          .WithEnvironment("Azure__Scope", scope)
          .WithEnvironment("Azure__ClientSecret", clientSecret)
          .WithEnvironment("EntraId__Instance", applicationInstance)
          .WithEnvironment("EntraId__TenantId", tenantId)
          .WithEnvironment("EntraId__ClientId", applicationId)
          .WithEnvironment("EntraId__Audience", applicationAudience);


var volcano = builder.CreateProject<Projects.DiscoverCostaRica_Volcano_Api>(Microservices.Volcano, azureSql, mongodb, geo)
          .WithEnvironment("Azure__TenantId", tenantId)
          .WithEnvironment("Azure__ClientId", clientId)
          .WithEnvironment("Azure__Scope", scope)
          .WithEnvironment("Azure__ClientSecret", clientSecret)
          .WithEnvironment("EntraId__Instance", applicationInstance)
          .WithEnvironment("EntraId__TenantId", tenantId)
          .WithEnvironment("EntraId__ClientId", applicationId)
          .WithEnvironment("EntraId__Audience", applicationAudience);


builder.AddYarp(Microservices.Gateway)
    .WithDeveloperCertificateTrust(true)
    .WithHttpsEndpoint(targetPort: 8080, name: "https")
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


