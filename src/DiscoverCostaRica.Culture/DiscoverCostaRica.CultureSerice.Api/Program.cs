using DiscoverCostaRica.Culture.Api.Extensions;
using DiscoverCostaRica.Culture.Api.Profiles;
using DiscoverCostaRica.Culture.Infraestructure.Context;
using DiscoverCostaRica.Culture.Infraestructure.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddDiscoverCostaRicaContext<ICultureContext, CultureContext>();
builder.AddMappingProfile<MappingProfile>();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapDefaultEndpoints();
app.MapCultureEndpoints();

await app.RunAsync();
