using DiscoverCostaRica.VolcanoService.Api.Extensions;
using DiscoverCostaRica.VolcanoService.Api.Profiles;
using DiscoverCostaRica.VolcanoService.Infraestructure.Context;
using DiscoverCostaRica.VolcanoService.Infraestructure.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddEntraIdAuthentication();
builder.AddDiscoverCostaRicaContext<IVolcanoContext, VolcanoContext>();
builder.AddMappingProfile<MappingProfile>();
builder.AddGlobalExeption();
builder.AddDiscoverCostaRicaLogger();
builder.AddVersioning();
builder.Services.AddOpenApi();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Volcano_Application();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Volcano_Infrastructure();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Shared();
builder.Services.AddDaprClient();
var app = builder.Build();

app.MapOpenApi();
app.AddScalar();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapVolcanoEndpoints();
app.MapDefaultEndpoints();
await app.RunAsync();