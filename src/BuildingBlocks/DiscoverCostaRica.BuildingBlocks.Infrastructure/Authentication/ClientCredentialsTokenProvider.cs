using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Options;

namespace DiscoverCostaRica.BuildingBlocks.Infrastructure.Authentication;

/// <summary>
/// OAuth 2.0 client-credentials tokens from Entra ID. Registered as a singleton so the credential,
/// and the token cache inside it, is shared by every request instead of rebuilt per call.
/// </summary>
public sealed class ClientCredentialsTokenProvider(IOptions<ClientCredentialsOptions> options) : IAccessTokenProvider
{
    private readonly ClientCredentialsOptions _options = options.Value;
    private readonly TokenCredential _credential = new ClientSecretCredential(
        options.Value.TenantId,
        options.Value.ClientId,
        options.Value.ClientSecret);

    public async Task<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        var token = await _credential.GetTokenAsync(new TokenRequestContext([_options.Scope]), cancellationToken);
        return token.Token;
    }
}
