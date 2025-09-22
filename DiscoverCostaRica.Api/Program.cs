using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using DiscoverCostaRica.Api.Configuration;
using DiscoverCostaRica.Api.Endpoints;
using DiscoverCostaRica.Api.ExtendedMethods;
using DiscoverCostaRica.Api.Middleware;
using DiscoverCostaRica.Api.Profiles;
using DiscoverCostaRica.Infraestructure.Data.Context;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                    new HeaderApiVersionReader("x-api-version"),
                                                    new MediaTypeApiVersionReader("x-api-version"));
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

//builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<RedisConfiguration>(options =>
{
    options.Endpoint = builder.Configuration["Cache:Endpoint"]!;
    options.Password = builder.Configuration["Cache:Password"]!;
});

builder.Services.AddDbContext<DiscoverCostaRicaContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DiscoverCostaRica"),
        options => options.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null));
}, ServiceLifetime.Singleton);

builder.Services.AddAutoMapper(typeof(AutomapperProfile));
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.RegisterServices();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});


var app = builder.Build();

app.MapDefaultEndpoints();


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
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseSwagger();
//app.UseSwaggerUI();
app.UseSwaggerUI(options =>
{
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
            description.GroupName.ToUpperInvariant());
    }
});


app.Run();

