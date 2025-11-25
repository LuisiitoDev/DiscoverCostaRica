using Dapr.Client;
using DiscoverCostaRica.Shared.Constants;
using DiscoverCostaRica.Volcano.Application.Dtos;
using DiscoverCostaRica.Volcano.Application.Interfaces;
using DiscoverCostaRica.VolcanoService.Domain.Models;

namespace DiscoverCostaRica.Volcano.Application.Services;

public class GeoMiroserviceClient(DaprClient dapr) : IGeoMiroserviceClient
{
    public Task<IDictionary<VolcanoModel, LocationDto>> GetVolcanoLocations(IEnumerable<VolcanoModel> volcanos, CancellationToken cancellationToken)
    {
        var locations = new Dictionary<VolcanoModel, LocationDto>();

        foreach (var volcano in volcanos)
        {
            var tasks = new List<Task>();

            dapr.GeoGet<object>($"api/Provinces/{volcano.ProvinceId}", cancellationToken);
            dapr.GeoGet<object>($"api/Canton/{volcano.CantonId}", cancellationToken);
            dapr.GeoGet<object>($"api/District/{volcano.DistrictId}", cancellationToken);
            dapr.InvokeMethodAsync<object>(HttpMethod.Get, Microservices.Geo, $"api/Provinces/{volcano.ProvinceId}");
            dapr.InvokeMethodAsync<object>(HttpMethod.Get, Microservices.Geo, $"api/Canton/{volcano.CantonId}");
            dapr.InvokeMethodAsync<object>(HttpMethod.Get, Microservices.Geo, $"api/District/{volcano.DistrictId}");
        }
    }
}

public static class GeoDaprExtensions
{
    private const string APP_ID = Microservices.Geo;

    public static async Task<TResult> GeoGet<TResult>(this DaprClient dapr, string endpoint, CancellationToken cancellationToken)
    {
        return await dapr.InvokeMethodAsync<TResult>(
            HttpMethod.Get,
            APP_ID, 
            endpoint,
            cancellationToken
        );
    }
}
