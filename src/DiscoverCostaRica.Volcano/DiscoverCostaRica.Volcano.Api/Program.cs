using DiscoverCostaRica.Volcano.Api.Endpoints;
using DiscoverCostaRica.Volcano.Infrastructure.Context;

// Composition root (Clean Architecture "Main"): the only place that wires concrete types.
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddEntraIdAuthentication();
builder.AddCatalogDbContext<VolcanoContext>();
builder.AddCatalogHybridCache();
builder.AddDiscoverCostaRicaLogger();
builder.AddVersioning();

builder.Services.AddOpenApi();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Volcano_Application();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Volcano_Infrastructure();
builder.Services.AddGeoLocationGateway();

var app = builder.Build();

app.MapOpenApi();
app.AddScalar();
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapVolcanoEndpoints();
app.MapDefaultEndpoints();

await app.RunAsync();
