using DiscoverCostaRica.Function.LogConsumer.Interfaces;
using DiscoverCostaRica.Function.LogConsumer.Services;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Services.AddSingleton<IQueueService, QueueService>();
builder.Services.AddSingleton<IMongoService, MongoService>();

builder.Build().Run();
