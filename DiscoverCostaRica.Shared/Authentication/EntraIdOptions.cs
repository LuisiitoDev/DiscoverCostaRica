namespace DiscoverCostaRica.Shared.Authentication;

/// <summary>
/// Configuration options for Microsoft Entra ID (Azure AD) authentication
/// </summary>
public class EntraIdOptions
{
    public const string SectionName = "EntraId";
    public required string Instance { get; set; }
    public required string TenantId { get; set; }
    public required string ClientId { get; set; }
    public required string Audience { get; set; }
}
