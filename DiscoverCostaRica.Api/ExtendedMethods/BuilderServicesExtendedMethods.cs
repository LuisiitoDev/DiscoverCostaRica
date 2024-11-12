using DiscoverCostaRica.Api.Services;
using DiscoverCostaRica.Api.Services.Crawler;

namespace DiscoverCostaRica.Api.ExtendedMethods;

public static class BuilderServicesExtendedMethods
{
	public static IServiceCollection RegisterServices(this IServiceCollection collection)
	{
		collection.AddScoped<BeachService>();
		collection.AddScoped<DirectionService>();
		collection.AddSingleton<BeachCrawlerService>();
		collection.AddSingleton<RedisCacheService>();
		collection.AddSingleton<DishService>();
		return collection;
	}	
}