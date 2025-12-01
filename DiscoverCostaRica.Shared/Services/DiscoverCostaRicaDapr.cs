using Dapr.Client;
using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Shared.Interfaces;
using DiscoverCostaRica.Shared.Responses;

namespace DiscoverCostaRica.Shared.Services;

[TransientService]
public class DiscoverCostaRicaDapr(DaprClient dapr) : IDiscoverCostaRicaDapr
{
    public async Task<TResult?> InvokeGetMethodAsync<TResult>(string appId, string endpoint, CancellationToken cancellationToken)
    {
        var result = await dapr.InvokeMethodAsync<Result<TResult>>(HttpMethod.Get, appId, endpoint, cancellationToken);
        return result.Value;
    }
}
