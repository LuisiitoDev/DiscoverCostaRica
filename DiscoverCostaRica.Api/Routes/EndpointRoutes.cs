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
		public const string GET_CANTONS = $"{API_ROUTE}/{VERSIONING}/Cantons";
		public const string GET_DISTRICTS = $"{API_ROUTE}/{VERSIONING}/Districts";
	}
}