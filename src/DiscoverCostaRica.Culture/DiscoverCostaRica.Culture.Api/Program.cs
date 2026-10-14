using DiscoverCostaRica.Culture.Api.Endpoints;
using DiscoverCostaRica.Culture.Infrastructure.Context;

// Composition root (Clean Architecture "Main"): the only place that wires concrete types.
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddEntraIdAuthentication();
builder.AddCatalogDbContext<CultureContext>();
builder.AddCatalogHybridCache();
builder.AddVersioning();

builder.Services.AddOpenApi();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Culture_Application();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Culture_Infrastructure();

var app = builder.Build();

app.MapOpenApi();
app.AddScalar();
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultEndpoints();
app.MapCultureEndpoints();

await app.RunAsync();
