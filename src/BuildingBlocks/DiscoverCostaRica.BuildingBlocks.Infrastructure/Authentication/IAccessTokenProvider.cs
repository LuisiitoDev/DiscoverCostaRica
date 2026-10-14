namespace DiscoverCostaRica.BuildingBlocks.Infrastructure.Authentication;

/// <summary>Supplies the bearer token for outbound service-to-service calls.</summary>
public interface IAccessTokenProvider
{
    Task<string> GetTokenAsync(CancellationToken cancellationToken);
}
