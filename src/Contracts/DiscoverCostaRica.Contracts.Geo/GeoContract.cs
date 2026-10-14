namespace DiscoverCostaRica.Contracts.Geo;

/// <summary>Routes and limits of the Geo v1 contract (specs/006-clean-architecture-migration/contracts).</summary>
public static class GeoContract
{
    /// <summary>Name the Geo service is registered under for service discovery (Aspire / Container Apps).</summary>
    public const string ServiceName = "geoservice";

    public const string ResolveLocationsRoute = "/api/v1/geo/locations:resolve";

    public const int MaxResolveBatchSize = 200;
}
