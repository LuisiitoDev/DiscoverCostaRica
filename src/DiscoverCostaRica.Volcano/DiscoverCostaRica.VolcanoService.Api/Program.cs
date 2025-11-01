using DiscoverCostaRica.VolcanoService.Api.Profiles;
using DiscoverCostaRica.VolcanoService.Application.Interfaces;
using DiscoverCostaRica.VolcanoService.Application.Services;
using DiscoverCostaRica.VolcanoService.Domain.Interfaces;
using DiscoverCostaRica.VolcanoService.Infraestructure.Context;
using DiscoverCostaRica.VolcanoService.Infraestructure.Interfaces;
using DiscoverCostaRica.VolcanoService.Infraestructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddDiscoverCostaRicaContext<VolcanoContext>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<VolcanoContext>(options =>
{
    options.UseSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DiscoverCostaRica")
            ?? throw new InvalidOperationException("Connection string 'DiscoverCostaRica' not found."),
        options => options.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null));
}, ServiceLifetime.Scoped);

builder.Services.AddScoped<IVolcanoContext>(provider => provider.GetRequiredService<VolcanoContext>());
builder.Services.AddTransient<IVolcanoRepository, VolcanoRepository>();
builder.Services.AddTransient<IVolcanoService, VolcanoService>();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();