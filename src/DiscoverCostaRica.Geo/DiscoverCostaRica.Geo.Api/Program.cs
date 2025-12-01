using DiscoverCostaRica.Geo.Api.Extensions;
using DiscoverCostaRica.Geo.Api.Profiles;
using DiscoverCostaRica.Geo.Infraestructure.Context;
using DiscoverCostaRica.Geo.Infraestructure.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddEntraIdAuthentication();
builder.AddDiscoverCostaRicaContext<IGeoContext, GeoContext>();
builder.AddMappingProfile<MappingProfile>();
builder.AddGlobalExeption();
builder.AddDiscoverCostaRicaLogger();
builder.AddVersioning();
builder.Services.AddOpenApi();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Geo_Application();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Geo_Infrastructure();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Shared();
builder.Services.AddDaprClient();
var app = builder.Build();

app.MapOpenApi();
app.AddScalar();


app.UseAuthentication();
app.UseAuthorization();
app.MapGeoEndpoints();
app.MapDefaultEndpoints();
await app.RunAsync();
