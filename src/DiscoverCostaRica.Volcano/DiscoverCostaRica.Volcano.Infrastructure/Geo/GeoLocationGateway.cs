using System.Globalization;
using System.Net.Http.Json;
using DiscoverCostaRica.Contracts.Geo;
using DiscoverCostaRica.Volcano.Application.Ports;
using DiscoverCostaRica.Volcano.Application.UseCases;
using DiscoverCostaRica.Volcano.Domain;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace DiscoverCostaRica.Volcano.Infrastructure.Geo;

/// <summary>
/// Resolves location names through Geo's batch contract: one call for every code not already cached,
/// instead of three calls per volcano (VOL-D4). Resolved names are cached per code for 24 hours, so
/// volcanoes keep their locations while Geo is down; a code never resolved is left out, never an error.
/// </summary>
internal sealed partial class GeoLocationGateway(HttpClient http, HybridCache cache, ILogger<GeoLocationGateway> logger)
    : IGeoLocationGateway
{
    public static readonly TimeSpan NameLifetime = TimeSpan.FromHours(24);

    private static readonly HybridCacheEntryOptions ReadOnly = new()
    {
        Flags = HybridCacheEntryFlags.DisableLocalCacheWrite | HybridCacheEntryFlags.DisableDistributedCacheWrite,
    };

    private static readonly HybridCacheEntryOptions Store = new() { Expiration = NameLifetime };

    public async Task<IReadOnlyDictionary<LocationCode, LocationResponse>> ResolveAsync(
        IReadOnlyCollection<LocationCode> codes,
        CancellationToken cancellationToken)
    {
        var resolved = new Dictionary<LocationCode, LocationResponse>();
        var missing = new List<LocationCode>();

        foreach (var code in codes.Distinct())
        {
            var cached = await cache.GetOrCreateAsync<LocationResponse?>(
                KeyFor(code), _ => ValueTask.FromResult<LocationResponse?>(null), ReadOnly, cancellationToken: cancellationToken);

            if (cached is null) missing.Add(code);
            else resolved[code] = cached;
        }

        foreach (var batch in missing.Chunk(GeoContract.MaxResolveBatchSize))
        {
            foreach (var (code, location) in await FetchAsync(batch, cancellationToken))
            {
                resolved[code] = location;
                await cache.SetAsync(KeyFor(code), location, Store, ["geo-names"], cancellationToken);
            }
        }

        return resolved;
    }

    private async Task<IEnumerable<(LocationCode Code, LocationResponse Location)>> FetchAsync(
        LocationCode[] codes,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new ResolveLocationsRequest([.. codes.Select(c => new LocationCodeDto(c.ProvinceId, c.CantonId, c.DistrictId))]);
            using var response = await http.PostAsJsonAsync(GeoContract.ResolveLocationsRoute, request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var envelope = await response.Content.ReadFromJsonAsync<GeoEnvelope<ResolveLocationsResponse>>(cancellationToken);
            var items = envelope?.Value?.Items ?? throw new InvalidOperationException("Geo answered without items.");

            return codes.Zip(items)
                .Where(pair => pair.Second.Province is not null || pair.Second.Canton is not null || pair.Second.District is not null)
                .Select(pair => (pair.First, ToLocation(pair.Second)))
                .ToList();
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            // The volcano catalog must keep serving while Geo is slow or down (spec 006 US3 #5).
            LogGeoUnavailable(logger, codes.Length, exception);
            return [];
        }
    }

    private static string KeyFor(LocationCode code) => string.Create(
        CultureInfo.InvariantCulture,
        $"volcano:location:{code.ProvinceId}:{code.CantonId}:{code.DistrictId}");

    private static LocationResponse ToLocation(ResolvedLocationDto dto) =>
        new(ToNamedRef(dto.Province), ToNamedRef(dto.Canton), ToNamedRef(dto.District));

    private static NamedRef? ToNamedRef(NamedRefDto? dto) => dto is null ? null : new NamedRef(dto.Id, dto.Name);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Geo could not resolve {Count} locations; serving volcanoes without them")]
    private static partial void LogGeoUnavailable(ILogger logger, int count, Exception exception);

    /// <summary>v1 response envelope, as Geo writes it.</summary>
    private sealed record GeoEnvelope<T>(T? Value, int StatusCode, string? Message);
}
