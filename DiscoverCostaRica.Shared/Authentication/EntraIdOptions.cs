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
    public string Instance { get; set; } = string.Empty;

    /// <summary>
    /// Azure AD Tenant ID
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Application (Client) ID
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// API audience identifier
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Comma-separated list of required scopes (e.g., "Beaches.Read,Beaches.Write")
    /// </summary>
    public string Scopes { get; set; } = string.Empty;

    /// <summary>
    /// Gets the authority URL for token validation
    /// </summary>
    public string Authority => $"{Instance.TrimEnd('/')}/{TenantId}";
}
