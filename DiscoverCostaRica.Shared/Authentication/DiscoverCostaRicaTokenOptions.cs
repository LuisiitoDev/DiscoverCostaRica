namespace DiscoverCostaRica.Shared.Authentication;

public class DiscoverCostaRicaTokenOptions
{
    public required string TenantId { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string Scope { get; set; }
}
