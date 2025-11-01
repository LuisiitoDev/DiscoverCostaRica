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


var app = builder.Build();

app.MapDefaultEndpoints();
app.MapBeachEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
