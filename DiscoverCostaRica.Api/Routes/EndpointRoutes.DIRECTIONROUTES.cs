namespace DiscoverCostaRica.Api.Routes;

public static partial class EndpointRoutes
{
    public static class DIRECTIONROUTES
	{
		public const string GET_PROVINCES = $"{API_ROUTE}/{VERSIONING}/Provinces";
		public const string GET_CANTONS = $"{API_ROUTE}/{VERSIONING}/{{provinceId}}/Cantons";
		public const string GET_DISTRICTS = $"{API_ROUTE}/{VERSIONING}/{{provinceId}}/{{cantonId}}/Districts";
	}
}