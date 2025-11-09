namespace DiscoverCostaRica.Shared.Routes;

public static class RoutesConstants
{
    public const string ApiPrefix = "api";
    public const string ApiVersionParameter = "v";

    public static class Geo
    {
        public const string Group = "geo";
        public const string Provinces = "/provinces";
        public const string Canton = "/cantons/{provinceId}";
        public const string Districts = "/districts/{cantonId}";
    }
}
