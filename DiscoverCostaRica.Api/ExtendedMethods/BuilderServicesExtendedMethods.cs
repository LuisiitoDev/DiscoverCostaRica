using DiscoverCostaRica.Api.Services;

namespace DiscoverCostaRica.Api.ExtendedMethods;

public static class BuilderServicesExtendedMethods
{
	public static IServiceCollection RegisterServices(this IServiceCollection collection)
	{
		collection.AddScoped<BeachService>();
		collection.AddScoped<DirectionService>();
		collection.AddSingleton<RedisCacheService>();
		collection.AddSingleton<DishService>();
		collection.AddSingleton<AttractionService>();
		collection.AddSingleton<CountryService>();
		return collection;
	}	
}