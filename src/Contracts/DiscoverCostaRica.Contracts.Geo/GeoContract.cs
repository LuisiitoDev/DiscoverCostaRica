namespace DiscoverCostaRica.Contracts.Geo;

/// <summary>Routes and limits of the Geo v1 contract (specs/006-clean-architecture-migration/contracts).</summary>
public static class GeoContract
{
    public const string ResolveLocationsRoute = "/api/v1/geo/locations:resolve";

    public const int MaxResolveBatchSize = 200;
}
