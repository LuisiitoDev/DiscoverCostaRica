using DiscoverCostaRica.Geo.Api.Extensions;
using DiscoverCostaRica.Geo.Api.Profiles;
using DiscoverCostaRica.Geo.Infraestructure.Context;
using DiscoverCostaRica.Geo.Infraestructure.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddDiscoverCostaRicaContext<IGeoContext, GeoContext>();
builder.AddMappingProfile<MappingProfile>();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapGeoEndpoints();
app.MapDefaultEndpoints();
await app.RunAsync();
