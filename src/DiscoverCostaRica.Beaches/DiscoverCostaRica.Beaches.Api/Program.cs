using DiscoverCostaRica.Beaches.Api.Extensions;
using DiscoverCostaRica.Beaches.Api.Profiles;
using DiscoverCostaRica.Beaches.Infrastructure.Context;
using DiscoverCostaRica.Beaches.Infrastructure.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddEntraIdAuthentication();
builder.AddDiscoverCostaRicaContext<IBeachContext, BeachContext>();
builder.AddMappingProfile<MappingProfile>();
builder.AddDiscoverCostaRicaLogger();
builder.AddVersioning();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Beaches_Application();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Beaches_Infrastructure();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Shared();

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
