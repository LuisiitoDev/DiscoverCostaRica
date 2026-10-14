namespace DiscoverCostaRica.BuildingBlocks.Infrastructure.Authentication;

/// <summary>Entra ID app registration used for service-to-service calls (configuration section <c>Azure</c>).</summary>
public sealed class ClientCredentialsOptions
{
    public const string SectionName = "Azure";

    public required string TenantId { get; set; }

    public required string ClientId { get; set; }

    public required string ClientSecret { get; set; }

    public required string Scope { get; set; }
}
