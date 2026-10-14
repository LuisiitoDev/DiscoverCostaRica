using DiscoverCostaRica.Geo.Api.Endpoints;
using DiscoverCostaRica.Geo.Infrastructure.Context;

// Composition root (Clean Architecture "Main"): the only place that wires concrete types.
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddEntraIdAuthentication();
builder.AddCatalogDbContext<GeoContext>();
builder.AddCatalogHybridCache();
builder.AddDiscoverCostaRicaLogger();
builder.AddVersioning();

builder.Services.AddOpenApi();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Geo_Application();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Geo_Infrastructure();

var app = builder.Build();

app.MapOpenApi();
app.AddScalar();
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapGeoEndpoints();
app.MapDefaultEndpoints();

await app.RunAsync();
