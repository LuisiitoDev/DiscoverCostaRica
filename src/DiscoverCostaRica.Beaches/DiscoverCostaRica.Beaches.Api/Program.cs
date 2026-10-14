using DiscoverCostaRica.Beaches.Api.Endpoints;
using DiscoverCostaRica.Beaches.Infrastructure.Context;

// Composition root (Clean Architecture "Main"): the only place that wires concrete types.
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddEntraIdAuthentication();
builder.AddCatalogDbContext<BeachContext>();
builder.AddCatalogHybridCache();
builder.AddVersioning();

builder.Services.AddOpenApi();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Beaches_Application();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Beaches_Infrastructure();

var app = builder.Build();

app.UseExceptionHandler();
app.MapOpenApi();
app.AddScalar();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultEndpoints();
app.MapBeachEndpoints();

await app.RunAsync();
