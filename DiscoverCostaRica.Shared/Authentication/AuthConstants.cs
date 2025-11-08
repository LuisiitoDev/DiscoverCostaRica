namespace DiscoverCostaRica.Shared.Authentication;

/// <summary>
/// Constants for authentication policies, roles, and claims
/// </summary>
public static class AuthConstants
{
    /// <summary>
    /// Authorization policy names
    /// </summary>
    public static class Policies
    {
        public const string BeachesRead = "Beaches.Read";
        public const string BeachesWrite = "Beaches.Write";
        public const string VolcanoRead = "Volcano.Read";
        public const string VolcanoWrite = "Volcano.Write";
        public const string CultureRead = "Culture.Read";
        public const string CultureWrite = "Culture.Write";
        public const string GeoRead = "Geo.Read";
        public const string GeoWrite = "Geo.Write";
    }

    /// <summary>
    /// Common role names
    /// </summary>
    public static class Roles
    {
        public const string Administrator = "Administrator";
        public const string User = "User";
        public const string Reader = "Reader";
    }

    /// <summary>
    /// Custom claim type names
    /// </summary>
    public static class ClaimTypes
    {
        public const string Scope = "http://schemas.microsoft.com/identity/claims/scope";
        public const string ObjectId = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        public const string TenantId = "http://schemas.microsoft.com/identity/claims/tenantid";
    }
}
