using DiscoverCostaRica.Beaches.Api.Extensions;
using DiscoverCostaRica.Beaches.Api.Profiles;
using DiscoverCostaRica.Beaches.Infrastructure.Context;
using DiscoverCostaRica.Beaches.Infrastructure.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddDiscoverCostaRicaContext<IBeachContext, BeachContext>();
builder.AddMappingProfile<MappingProfile>();
builder.AddGlobalExeption();
builder.AddFunctionLogger();
builder.AddVersioning();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Beaches_Application();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Beaches_Infrastructure();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Shared();
builder.Services.AddDaprClient();

var app = builder.Build();

app.MapOpenApi();
app.AddScalar();
app.UseHttpsRedirection();
app.MapDefaultEndpoints();
app.MapBeachEndpoints();
await app.RunAsync();
