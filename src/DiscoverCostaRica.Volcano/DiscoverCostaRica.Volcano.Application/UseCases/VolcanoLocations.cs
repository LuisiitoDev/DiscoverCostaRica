using DiscoverCostaRica.Volcano.Application.Ports;

namespace DiscoverCostaRica.Volcano.Application.UseCases;

/// <summary>Builds responses with locations, resolving every distinct code in one gateway call (VOL-D4).</summary>
internal static class VolcanoLocations
{
    public static async Task<List<VolcanoResponse>> WithLocationsAsync(
        this IGeoLocationGateway gateway,
        IReadOnlyList<Domain.Volcano> volcanoes,
        CancellationToken cancellationToken)
    {
        var codes = volcanoes.Select(v => v.Location).Distinct().ToList();
        var locations = await gateway.ResolveAsync(codes, cancellationToken);

        return [.. volcanoes.Select(v => new VolcanoResponse(v.Id, v.Name, v.Description, locations.GetValueOrDefault(v.Location)))];
    }
}
