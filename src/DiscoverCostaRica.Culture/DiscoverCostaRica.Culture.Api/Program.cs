using DiscoverCostaRica.Culture.Api.Extensions;
using DiscoverCostaRica.Culture.Api.Profiles;
using DiscoverCostaRica.Culture.Infraestructure.Context;
using DiscoverCostaRica.Culture.Infraestructure.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddEntraIdAuthentication();
builder.AddDiscoverCostaRicaContext<ICultureContext, CultureContext>();
builder.AddMappingProfile<MappingProfile>();
builder.AddGlobalExeption();
builder.AddDiscoverCostaRicaLogger();
builder.AddVersioning();
builder.Services.AddOpenApi();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Culture_Application();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Culture_Infrastructure();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Shared();
builder.Services.AddDaprClient();

var app = builder.Build();

app.MapOpenApi();
app.AddScalar();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultEndpoints();
app.MapCultureEndpoints();

await app.RunAsync();
