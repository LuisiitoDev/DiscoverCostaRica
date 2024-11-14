namespace DiscoverCostaRica.Api.Routes;

public static class EndpointRoutes
{
	public const string API_ROUTE = "/api";
	public const string VERSIONING = "v1";
	public static class BeachRoutes
	{
		public const string GET_BEACHES = $"{API_ROUTE}/{VERSIONING}/Beaches";
	}
	public static class DIRECTIONROUTES
	{
		public const string GET_PROVINCES = $"{API_ROUTE}/{VERSIONING}/Provinces";
		public const string GET_CANTONS = $"{API_ROUTE}/{VERSIONING}/{{provinceId}}/Cantons";
		public const string GET_DISTRICTS = $"{API_ROUTE}/{VERSIONING}/{{provinceId}}/{{cantonId}}/Districts";
	}

	public static class ATTRACTIONS_ROUTE
	{
		public const string GET_ATTRACTIONS = $"{API_ROUTE}/{VERSIONING}/Attractions";
	}

	public static class DISH_ROUTES
	{
		public const string GET_DISHES = $"{API_ROUTE}/{VERSIONING}/Dish";
	}
}