using DiscoverCostaRica.Beaches.Api.Extensions;
using DiscoverCostaRica.Beaches.Api.Profiles;
using DiscoverCostaRica.Beaches.Infraestructure.Context;
using DiscoverCostaRica.Beaches.Infraestructure.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddDiscoverCostaRicaContext<IBeachContext, BeachContext>();
builder.AddMappingProfile<MappingProfile>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Beaches_Application();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Beaches_Infraestructure();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapDefaultEndpoints();
app.MapBeachEndpoints();

await app.RunAsync();
