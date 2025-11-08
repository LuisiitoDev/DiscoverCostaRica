namespace DiscoverCostaRica.Shared.Authentication;

/// <summary>
/// Constants for authentication policies, roles, and claims
/// </summary>
public static partial class AuthConstants
{
    /// <summary>
    /// Authorization policy names
    /// </summary>
    public static class Policies
    {
        public const string BeachesRead = "Beaches.Read";
        public const string VolcanoRead = "Volcano.Read";
        public const string CultureRead = "Culture.Read";
        public const string GeoRead = "Geo.Read";
    }
}
