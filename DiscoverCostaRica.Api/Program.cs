using System.Reflection;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using DiscoverCostaRica.Api.Endpoints;
using DiscoverCostaRica.Infraestructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using DiscoverCostaRica.Api.ExtendedMethods;
using DiscoverCostaRica.Api.BackgroundServices;
using DiscoverCostaRica.Api.Configuration;

// code ~/.microsoft/usersecrets/30d2aea7-df7e-4cda-a366-079494c613ba/secrets.json


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RedisConfiguration>(options => 
{
	options.Endpoint = "redis-10214.c282.east-us-mz.azure.redns.redis-cloud.com:10214";
	options.Password = "ngCHlowKobEcoiasFSdvxX4FqqRHvOEO";
});

builder.Services.AddDbContext<DiscoverCostaRicaContext>(options =>
{
	options.UseSqlServer(builder.Configuration["ConnectionStrings:DiscoverCostaRica"]);
}, ServiceLifetime.Singleton);

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

