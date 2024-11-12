using System.Reflection;
using DiscoverCostaRica.Api.Endpoints;
using DiscoverCostaRica.Infraestructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using DiscoverCostaRica.Api.ExtendedMethods;
using DiscoverCostaRica.Api.Configuration;
using DiscoverCostaRica.Api.Profiles;
using Microsoft.Extensions.Logging.ApplicationInsights;
using DiscoverCostaRica.Api.Middleware;
using DiscoverCostaRica.Infraestructure.Services;

// code ~/.microsoft/usersecrets/0c1b65b8-5105-468c-9773-f8b1dc7fc846/secrets.json


var builder = WebApplication.CreateBuilder(args);

var credentialPath = Path.Combine(AppContext.BaseDirectory, "service_account_key.json");
var secretManager = new SecretManagerService("discovercostarica",credentialPath);

var connectionString = secretManager.GetSecret("DiscoverCostaRicaDB");
var applicationInsights = secretManager.GetSecret("DiscoverCostaRicaApplicationInsights");
var redisEndpoint = secretManager.GetSecret("RedisEndpoint");
var redispassword = secretManager.GetSecret("RedisPassword");

builder.Logging.AddApplicationInsights(
	configureTelemetryConfiguration: config => 
	 	config.ConnectionString = applicationInsights,
	configureApplicationInsightsLoggerOptions: options => {}
);

builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("error", LogLevel.Error);

builder.Services.Configure<RedisConfiguration>(options => {
	options.Endpoint = redisEndpoint;
	options.Endpoint = redispassword;
});

builder.Services.AddDbContext<DiscoverCostaRicaContext>(options =>
{
	options.UseSqlServer(connectionString);
}, ServiceLifetime.Singleton);

builder.Services.AddAutoMapper(typeof(AutomapperProfile));
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.RegisterServices();
//builder.Services.AddHostedService<BeachBackgroundService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
// Mapping endpoints
var endpoints = Assembly
.GetExecutingAssembly()
.GetTypes()
.Where(type => type.GetInterfaces().Contains(typeof(IEndpoint)));


foreach (var endpoint in endpoints)
{
	var method = endpoint.GetMethod(nameof(IEndpoint.Register));
	method?.Invoke(null, [app]);
}

// Mapping endpoints

app.Run();

