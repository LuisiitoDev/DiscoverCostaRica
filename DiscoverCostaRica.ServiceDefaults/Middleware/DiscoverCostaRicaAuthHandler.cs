using DiscoverCostaRica.Shared.Interfaces;
using System.Net.Http.Headers;

namespace DiscoverCostaRica.ServiceDefaults.Middleware;

public class DiscoverCostaRicaAuthHandler(IDiscoverCostaRicaTokenAcquisitionService discoverCostaRicaToken) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await discoverCostaRicaToken.GetTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
}
