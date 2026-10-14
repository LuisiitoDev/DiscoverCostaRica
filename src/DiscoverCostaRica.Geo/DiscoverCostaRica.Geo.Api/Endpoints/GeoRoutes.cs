namespace DiscoverCostaRica.Geo.Api.Endpoints;

/// <summary>Geo v1 routes, relative to <c>/api/v1/geo</c> (contracts/public-api-v1.md § Geo).</summary>
internal static class GeoRoutes
{
    public const string Group = "geo";

    public const string Provinces = "/provinces";
    public const string ProvinceById = "/provinces/{provinceId}";

    // Province-scoped canton routes (as built).
    public const string CantonsOfProvince = "/cantons/{provinceId}";
    public const string CantonById = "/cantons/{provinceId}/{cantonId}";

    // Deprecated: a canton number alone does not identify a canton (GEO-D2, GEO-D3).
    public const string DistrictsOfCanton = "/districts/{cantonId}";
    public const string DistrictById = "/districts/{cantonId}/{districtId}";

    // Hierarchical successors (spec 006 US2, additive).
    public const string ProvinceCantons = "/provinces/{provinceId:int}/cantons";
    public const string ProvinceCanton = "/provinces/{provinceId:int}/cantons/{cantonId:int}";
    public const string CantonDistricts = "/provinces/{provinceId:int}/cantons/{cantonId:int}/districts";
    public const string CantonDistrict = "/provinces/{provinceId:int}/cantons/{cantonId:int}/districts/{districtId:int}";

    // Internal batch contract (Contracts.Geo).
    public const string ResolveLocations = "/locations:resolve";
}
