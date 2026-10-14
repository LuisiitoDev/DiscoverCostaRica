namespace DiscoverCostaRica.Volcano.Application.UseCases.GetVolcanoes;

public sealed record GetVolcanoesRequest
{
    public static GetVolcanoesRequest Instance { get; } = new();
}
