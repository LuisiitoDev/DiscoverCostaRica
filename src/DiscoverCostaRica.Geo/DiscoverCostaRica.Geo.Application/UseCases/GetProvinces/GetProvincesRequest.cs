namespace DiscoverCostaRica.Geo.Application.UseCases.GetProvinces;

public sealed record GetProvincesRequest
{
    public static GetProvincesRequest Instance { get; } = new();
}
