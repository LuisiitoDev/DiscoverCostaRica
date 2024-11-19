using DiscoverCostaRica.Functions.Configuration;
using DiscoverCostaRica.Functions.Services;
using DiscoverCostaRica.Infraestructure.Data.Context;
using DiscoverCostaRica.Infraestructure.Services;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

var credentialPath = Path.Combine(AppContext.BaseDirectory, "service_account_key.json");
var secretManager = new SecretManagerService("discovercostarica", credentialPath);

builder.Services.Configure<CacheConfiguration>(options =>
{
    options.Endpoint = secretManager.GetSecret("RedisEndpoint");
    options.Password = secretManager.GetSecret("RedisPassword");
});

builder.Services.AddDbContext<DiscoverCostaRicaContext>(options =>
{
    options.UseSqlServer(
        secretManager.GetSecret("DiscoverCostaRicaDB"),
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
