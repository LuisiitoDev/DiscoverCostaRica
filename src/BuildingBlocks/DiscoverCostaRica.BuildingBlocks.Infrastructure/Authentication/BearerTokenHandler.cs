using System.Net.Http.Headers;

namespace DiscoverCostaRica.BuildingBlocks.Infrastructure.Authentication;

/// <summary>Adds the service-to-service bearer token to outbound requests.</summary>
public sealed class BearerTokenHandler(IAccessTokenProvider tokenProvider) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await tokenProvider.GetTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
}
