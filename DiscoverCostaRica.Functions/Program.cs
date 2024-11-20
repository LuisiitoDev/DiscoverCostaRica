using DiscoverCostaRica.Functions.Configuration;
using DiscoverCostaRica.Functions.Services;
using DiscoverCostaRica.Infraestructure.Data.Context;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

string redisEndpoint = builder.Configuration["Redis:Endpoint"]!;
string redisPassword = builder.Configuration["Redis:Password"]!;
string connectionString = builder.Configuration["DiscoverCostaRica"]!;

builder.Services.Configure<CacheConfiguration>(options =>
{
    options.Endpoint = redisEndpoint;
    options.Password = redisPassword;
});

builder.Services.AddDbContext<DiscoverCostaRicaContext>(options =>
{
    options.UseSqlServer(
        connectionString,
        options => options.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null));
}, ServiceLifetime.Singleton);

builder.Services.AddSingleton<CacheService>();
builder.Services.AddSingleton<BeachService>();
builder.Services.AddSingleton<DishService>();
builder.Services.AddSingleton<ProvinceService>();
builder.Services.AddSingleton<VolcanoService>();

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
