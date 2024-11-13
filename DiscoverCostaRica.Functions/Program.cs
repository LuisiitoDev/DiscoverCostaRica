using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DiscoverCostaRica.Infraestructure.Services;

var credentialPath = Path.Combine(AppContext.BaseDirectory, "service_account_key.json");
var secretManager = new SecretManagerService("discovercostarica",credentialPath);

var connectionString = secretManager.GetSecret("DiscoverCostaRicaDB");

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();