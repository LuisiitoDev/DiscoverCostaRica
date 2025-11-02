using DiscoverCostaRica.VolcanoService.Api.Extensions;
using DiscoverCostaRica.VolcanoService.Api.Profiles;
using DiscoverCostaRica.VolcanoService.Infraestructure.Context;
using DiscoverCostaRica.VolcanoService.Infraestructure.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddDiscoverCostaRicaContext<IVolcanoContext, VolcanoContext>();
builder.AddMappingProfile<MappingProfile>();
builder.AddGlobalExeption();
builder.AddRabbitMq("LoggerMessaging").AddRabbitMqLoggerProvider();
builder.Services.AddOpenApi();
builder.Services.AddGeneratedServices_DiscoverCostaRica_VolcanoService_Application();
builder.Services.AddGeneratedServices_DiscoverCostaRica_VolcanoService_Infraestructure();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapVolcanoEndpoints();
app.MapDefaultEndpoints();
app.UseHttpsRedirection();
app.UseExceptionHandler();
await app.RunAsync();