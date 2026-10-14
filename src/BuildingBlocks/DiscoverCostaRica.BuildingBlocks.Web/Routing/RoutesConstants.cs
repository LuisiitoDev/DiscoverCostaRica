namespace DiscoverCostaRica.BuildingBlocks.Web.Routing;

public static class RoutesConstants
{
    public const string ApiPrefix = "api";
    public const string ApiVersionParameter = "v";

    public static class Beaches
    {
        public const string Group = "beaches";
        public const string Beach = "/";
    }

    public static class Traditions
    {
        public const string Group = "traditions";
        public const string Tradition = "/tradition";
        public const string Dish = "/dish";
    }

    public static class Volcanoes
    {
        public const string Group = "volcanoes";
        public const string Volcano = "/";
        public const string ByProvince = "/province/{provinceId}";
        public const string ById = "/{id}";
    }
}
