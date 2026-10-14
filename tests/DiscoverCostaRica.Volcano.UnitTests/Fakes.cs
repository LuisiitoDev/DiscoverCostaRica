using DiscoverCostaRica.Volcano.Application.Ports;
using DiscoverCostaRica.Volcano.Application.UseCases;
using DiscoverCostaRica.Volcano.Domain;

namespace DiscoverCostaRica.Volcano.UnitTests;

internal sealed class FakeVolcanoReadRepository : IVolcanoReadRepository
{
    public List<Domain.Volcano> Volcanoes { get; } =
    [
        Domain.Volcano.Create(1, "Arenal", "Stratovolcano.", LocationCode.Create(2, 10, 7)),
        Domain.Volcano.Create(2, "Poás", "Crater lake.", LocationCode.Create(2, 8)),
        Domain.Volcano.Create(3, "Barva", "Same canton as Poás for the test.", LocationCode.Create(2, 8)),
    ];

    public Task<IReadOnlyList<Domain.Volcano>> GetAllAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Domain.Volcano>>(Volcanoes);

    public Task<Domain.Volcano?> FindAsync(int id, CancellationToken cancellationToken) =>
        Task.FromResult(Volcanoes.SingleOrDefault(v => v.Id == id));

    public Task<IReadOnlyList<Domain.Volcano>> GetByProvinceAsync(int provinceId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Domain.Volcano>>([.. Volcanoes.Where(v => v.Location.ProvinceId == provinceId)]);
}

internal sealed class FakeGeoLocationGateway : IGeoLocationGateway
{
    public List<IReadOnlyCollection<LocationCode>> Calls { get; } = [];

    public Dictionary<LocationCode, LocationResponse> Known { get; } = new()
    {
        [LocationCode.Create(2, 10, 7)] = new(new(2, "Alajuela"), new(10, "San Carlos"), new(7, "La Fortuna")),
    };

    public Task<IReadOnlyDictionary<LocationCode, LocationResponse>> ResolveAsync(IReadOnlyCollection<LocationCode> codes, CancellationToken cancellationToken)
    {
        Calls.Add(codes);
        return Task.FromResult<IReadOnlyDictionary<LocationCode, LocationResponse>>(
            Known.Where(k => codes.Contains(k.Key)).ToDictionary());
    }
}
