namespace DiscoverCostaRica.Shared.Authentication;

/// <summary>
/// Configuration options for Microsoft Entra ID (Azure AD) authentication
/// </summary>
public class EntraIdOptions
{
    public const string SectionName = "EntraId";

    /// <summary>
    /// Azure AD instance (e.g., https://login.microsoftonline.com/)
    /// </summary>
    public required string Instance { get; set; }

    /// <summary>
    /// Azure AD Tenant ID
    /// </summary>
    public required string TenantId { get; set; }

    /// <summary>
    /// Application (Client) ID
    /// </summary>
    public required string ClientId { get; set; }

    /// <summary>
    /// API audience identifier
    /// </summary>
    public required string Audience { get; set; }

    /// <summary>
    /// Comma-separated list of required scopes (e.g., "Beaches.Read,Beaches.Write")
    /// </summary>
    public required string Scopes { get; set; }

    /// <summary>
    /// Gets the authority URL for token validation
    /// </summary>
    public string Authority => $"{Instance.TrimEnd('/')}/{TenantId}";
}
