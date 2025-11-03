using DiscoverCostaRica.Culture.Api.Extensions;
using DiscoverCostaRica.Culture.Api.Profiles;
using DiscoverCostaRica.Culture.Infraestructure.Context;
using DiscoverCostaRica.Culture.Infraestructure.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddDiscoverCostaRicaContext<ICultureContext, CultureContext>();
builder.AddMappingProfile<MappingProfile>();
builder.AddGlobalExeption();
builder.AddRabbitMq("LoggerMessaging").AddEventGridLogger();
builder.Services.AddOpenApi();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Culture_Application();
builder.Services.AddGeneratedServices_DiscoverCostaRica_Culture_Infraestructure();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapDefaultEndpoints();
app.MapCultureEndpoints();
app.UseExceptionHandler();
await app.RunAsync();
