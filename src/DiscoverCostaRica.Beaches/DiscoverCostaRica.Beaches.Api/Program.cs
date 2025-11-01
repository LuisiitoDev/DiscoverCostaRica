using DiscoverCostaRica.Beaches.Api.Extensions;
using DiscoverCostaRica.Beaches.Api.Profiles;
using DiscoverCostaRica.Beaches.Application.Interfaces;
using DiscoverCostaRica.Beaches.Application.Services;
using DiscoverCostaRica.Beaches.Domain.Interfaces;
using DiscoverCostaRica.Beaches.Infraestructure.Context;
using DiscoverCostaRica.Beaches.Infraestructure.Interfaces;
using DiscoverCostaRica.Beaches.Infraestructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddDiscoverCostaRicaContext<BeachContext>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBeachContext>(provider => provider.GetRequiredService<BeachContext>());
builder.Services.AddTransient<IBeachRepository, BeachRepository>();
builder.Services.AddTransient<IBeachService, BeachService>();

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

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
