namespace DiscoverCostaRica.Shared.Authentication;

public static partial class AuthConstants
{
    /// <summary>
    /// Custom claim type names
    /// </summary>
    public static class ClaimTypes
    {
        public const string Scope = "http://schemas.microsoft.com/identity/claims/scope";
        public const string ScopeShort = "scp";
        public const string ObjectId = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        public const string TenantId = "http://schemas.microsoft.com/identity/claims/tenantid";
    }
}
