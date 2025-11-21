using Azure.Core;
using Azure.Identity;

namespace DiscoverCostaRica.Tests.Infraestructure;

public class AzureEntraHandler(string tenantId, string clientId, string clientSecret, string scope) : DelegatingHandler
{
    private readonly TokenCredential _credential = new ClientSecretCredential(tenantId, clientId, clientSecret);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var tokenRequestContext = new TokenRequestContext([scope]);
        var accessToken = await _credential.GetTokenAsync(tokenRequestContext, cancellationToken);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken.Token);
        return await base.SendAsync(request, cancellationToken);
    }
}
