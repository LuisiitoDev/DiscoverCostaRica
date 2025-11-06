using DiscoverCostaRica.VolcanoService.Api.Extensions;
using DiscoverCostaRica.VolcanoService.Api.Profiles;
using DiscoverCostaRica.VolcanoService.Infraestructure.Context;
using DiscoverCostaRica.VolcanoService.Infraestructure.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddDiscoverCostaRicaContext<IVolcanoContext, VolcanoContext>();
builder.AddMappingProfile<MappingProfile>();
builder.AddGlobalExeption();
builder.AddFunctionLogger();
builder.Services.AddOpenApi();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Volcano_Application();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Volcano_Infrastructure();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Shared();
builder.Services.AddDaprClient();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapVolcanoEndpoints();
app.MapDefaultEndpoints();
app.UseHttpsRedirection();
await app.RunAsync();