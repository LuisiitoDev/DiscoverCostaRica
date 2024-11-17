using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DiscoverCostaRica.Infraestructure.Services;
using DiscoverCostaRica.Infraestructure.Data.Context;
using Microsoft.EntityFrameworkCore;

var credentialPath = Path.Combine(AppContext.BaseDirectory, "service_account_key.json");
var secretManager = new SecretManagerService("discovercostarica", credentialPath);

var connectionString = secretManager.GetSecret("DiscoverCostaRicaDB");

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddDbContext<DiscoverCostaRicaContext>(options =>
        {
            options.UseSqlServer(connectionString);
        }, ServiceLifetime.Singleton);
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();