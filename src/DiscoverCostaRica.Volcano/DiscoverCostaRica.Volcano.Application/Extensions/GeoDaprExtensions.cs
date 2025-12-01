using Dapr.Client;
using DiscoverCostaRica.Shared.Constants;

namespace DiscoverCostaRica.Volcano.Application.Extensions;

internal static class GeoDaprExtensions
{
    private const string APP_ID = Microservices.Geo;
    public static async Task<TResult> Get<TResult>(this DaprClient client, string endpoint, CancellationToken cancellationToken)
    {
        return await client.InvokeMethodAsync<TResult>(
            HttpMethod.Get,
            APP_ID,
            endpoint,
            cancellationToken
        );
    }
}
