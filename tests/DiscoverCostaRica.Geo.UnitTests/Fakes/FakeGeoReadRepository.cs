using DiscoverCostaRica.Geo.Application.Ports;
using DiscoverCostaRica.Geo.Domain;
using DiscoverCostaRica.Geo.Domain.ValueObjects;

namespace DiscoverCostaRica.Geo.UnitTests.Fakes;

/// <summary>In-memory port. Canton 1 exists in provinces 1 and 2 on purpose (codes are only unique inside their parent).</summary>
internal sealed class FakeGeoReadRepository : IGeoReadRepository
{
    public List<Province> Provinces { get; } =
    [
        Province.Create(ProvinceCode.Create(1), "San José"),
        Province.Create(ProvinceCode.Create(2), "Alajuela"),
    ];

    public List<Canton> Cantons { get; } =
    [
        Canton.Create(CantonCode.Create(ProvinceCode.Create(1), 1), "San José"),
        Canton.Create(CantonCode.Create(ProvinceCode.Create(1), 2), "Escazú"),
        Canton.Create(CantonCode.Create(ProvinceCode.Create(2), 1), "Alajuela"),
    ];

    public List<District> Districts { get; } =
    [
        District.Create(DistrictCode.Create(CantonCode.Create(ProvinceCode.Create(1), 1), 1), "Carmen"),
        District.Create(DistrictCode.Create(CantonCode.Create(ProvinceCode.Create(1), 1), 2), "Merced"),
        District.Create(DistrictCode.Create(CantonCode.Create(ProvinceCode.Create(2), 1), 1), "Alajuela"),
    ];

    public int BatchQueries { get; private set; }

    public Task<IReadOnlyList<Province>> GetProvincesAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Province>>(Provinces);

    public Task<Province?> FindProvinceAsync(ProvinceCode code, CancellationToken cancellationToken) =>
        Task.FromResult(Provinces.SingleOrDefault(p => p.Code == code));

    public Task<IReadOnlyList<Canton>> GetCantonsAsync(ProvinceCode province, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Canton>>([.. Cantons.Where(c => c.Code.Province == province)]);

    public Task<Canton?> FindCantonAsync(CantonCode code, CancellationToken cancellationToken) =>
        Task.FromResult(Cantons.SingleOrDefault(c => c.Code == code));

    public Task<IReadOnlyList<District>> GetDistrictsAsync(CantonCode canton, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<District>>([.. Districts.Where(d => d.Code.Canton == canton)]);

    public Task<District?> FindDistrictAsync(DistrictCode code, CancellationToken cancellationToken) =>
        Task.FromResult(Districts.SingleOrDefault(d => d.Code == code));

    public Task<IReadOnlyList<District>> GetDistrictsInAnyProvinceAsync(int canton, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<District>>([.. Districts.Where(d => d.Code.Canton.Canton == canton)]);

    public Task<District?> FindDistrictInAnyProvinceAsync(int canton, int district, CancellationToken cancellationToken) =>
        Task.FromResult(Districts
            .Where(d => d.Code.Canton.Canton == canton && d.Code.District == district)
            .OrderBy(d => d.Code.Canton.Province.Value)
            .FirstOrDefault());

    public Task<IReadOnlyList<Province>> GetProvincesByCodeAsync(IReadOnlyCollection<ProvinceCode> codes, CancellationToken cancellationToken)
    {
        BatchQueries++;
        return Task.FromResult<IReadOnlyList<Province>>([.. Provinces.Where(p => codes.Contains(p.Code))]);
    }

    public Task<IReadOnlyList<Canton>> GetCantonsByCodeAsync(IReadOnlyCollection<CantonCode> codes, CancellationToken cancellationToken)
    {
        BatchQueries++;
        return Task.FromResult<IReadOnlyList<Canton>>([.. Cantons.Where(c => codes.Contains(c.Code))]);
    }

    public Task<IReadOnlyList<District>> GetDistrictsByCodeAsync(IReadOnlyCollection<DistrictCode> codes, CancellationToken cancellationToken)
    {
        BatchQueries++;
        return Task.FromResult<IReadOnlyList<District>>([.. Districts.Where(d => codes.Contains(d.Code))]);
    }
}
