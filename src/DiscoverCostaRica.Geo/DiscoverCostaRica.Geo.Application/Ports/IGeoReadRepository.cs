using DiscoverCostaRica.Geo.Domain;
using DiscoverCostaRica.Geo.Domain.ValueObjects;

namespace DiscoverCostaRica.Geo.Application.Ports;

/// <summary>Read access to the administrative divisions, as the Geo use cases need it. Lists come ordered by code.</summary>
public interface IGeoReadRepository
{
    Task<IReadOnlyList<Province>> GetProvincesAsync(CancellationToken cancellationToken);

    Task<Province?> FindProvinceAsync(ProvinceCode code, CancellationToken cancellationToken);

    Task<IReadOnlyList<Canton>> GetCantonsAsync(ProvinceCode province, CancellationToken cancellationToken);

    Task<Canton?> FindCantonAsync(CantonCode code, CancellationToken cancellationToken);

    Task<IReadOnlyList<District>> GetDistrictsAsync(CantonCode canton, CancellationToken cancellationToken);

    Task<District?> FindDistrictAsync(DistrictCode code, CancellationToken cancellationToken);

    /// <summary>Deprecated v1 route: districts of every canton with this number, whatever its province (GEO-D2).</summary>
    Task<IReadOnlyList<District>> GetDistrictsInAnyProvinceAsync(int canton, CancellationToken cancellationToken);

    /// <summary>Deprecated v1 route: the first district with these numbers, lowest province first (GEO-D3).</summary>
    Task<District?> FindDistrictInAnyProvinceAsync(int canton, int district, CancellationToken cancellationToken);

    /// <summary>Set-based lookups for batch resolution: one query per level, whatever the number of codes.</summary>
    Task<IReadOnlyList<Province>> GetProvincesByCodeAsync(IReadOnlyCollection<ProvinceCode> codes, CancellationToken cancellationToken);

    Task<IReadOnlyList<Canton>> GetCantonsByCodeAsync(IReadOnlyCollection<CantonCode> codes, CancellationToken cancellationToken);

    Task<IReadOnlyList<District>> GetDistrictsByCodeAsync(IReadOnlyCollection<DistrictCode> codes, CancellationToken cancellationToken);
}
