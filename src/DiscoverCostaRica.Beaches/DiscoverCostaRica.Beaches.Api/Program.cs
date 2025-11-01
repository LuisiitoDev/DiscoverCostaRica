using DiscoverCostaRica.Beaches.Api.Extensions;
using DiscoverCostaRica.Beaches.Api.Profiles;
using DiscoverCostaRica.Beaches.Application.Interfaces;
using DiscoverCostaRica.Beaches.Application.Services;
using DiscoverCostaRica.Beaches.Domain.Interfaces;
using DiscoverCostaRica.Beaches.Infraestructure.Context;
using DiscoverCostaRica.Beaches.Infraestructure.Interfaces;
using DiscoverCostaRica.Beaches.Infraestructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BeachContext>(options =>
{
    options.UseSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DiscoverCostaRica") 
            ?? throw new InvalidOperationException("Connection string 'DiscoverCostaRica' not found."),
        options => options.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null));
}, ServiceLifetime.Scoped);

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
