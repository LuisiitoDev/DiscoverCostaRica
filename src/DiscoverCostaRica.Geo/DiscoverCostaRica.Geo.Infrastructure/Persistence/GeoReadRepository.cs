using DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection;
using DiscoverCostaRica.Geo.Application.Ports;
using DiscoverCostaRica.Geo.Domain;
using DiscoverCostaRica.Geo.Domain.ValueObjects;
using DiscoverCostaRica.Geo.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Geo.Infrastructure.Persistence;

/// <summary>EF Core gateway for <see cref="IGeoReadRepository"/>. Read-only, untracked, ordered by code.</summary>
[TransientService]
internal sealed class GeoReadRepository(GeoContext context) : IGeoReadRepository
{
    private IQueryable<ProvinceRow> Provinces => context.Set<ProvinceRow>().AsNoTracking();

    private IQueryable<CantonRow> Cantons => context.Set<CantonRow>().AsNoTracking();

    private IQueryable<DistrictRow> Districts => context.Set<DistrictRow>().AsNoTracking();

    public async Task<IReadOnlyList<Province>> GetProvincesAsync(CancellationToken cancellationToken) =>
        ToProvinces(await Provinces.OrderBy(p => p.Id).ToListAsync(cancellationToken));

    public async Task<Province?> FindProvinceAsync(ProvinceCode code, CancellationToken cancellationToken)
    {
        var row = await Provinces.FirstOrDefaultAsync(p => p.Id == code.Value, cancellationToken);
        return row is null ? null : ToProvince(row);
    }

    public async Task<IReadOnlyList<Canton>> GetCantonsAsync(ProvinceCode province, CancellationToken cancellationToken) =>
        ToCantons(await Cantons
            .Where(c => c.ProvinceId == province.Value)
            .OrderBy(c => c.Id)
            .ToListAsync(cancellationToken));

    public async Task<Canton?> FindCantonAsync(CantonCode code, CancellationToken cancellationToken)
    {
        var row = await Cantons.FirstOrDefaultAsync(c => c.ProvinceId == code.Province.Value && c.Id == code.Canton, cancellationToken);
        return row is null ? null : ToCanton(row);
    }

    public async Task<IReadOnlyList<District>> GetDistrictsAsync(CantonCode canton, CancellationToken cancellationToken) =>
        ToDistricts(await Districts
            .Where(d => d.CantonProvinceId == canton.Province.Value && d.CantonId == canton.Canton)
            .OrderBy(d => d.Id)
            .ToListAsync(cancellationToken));

    public async Task<District?> FindDistrictAsync(DistrictCode code, CancellationToken cancellationToken)
    {
        var row = await Districts.FirstOrDefaultAsync(
            d => d.CantonProvinceId == code.Canton.Province.Value && d.CantonId == code.Canton.Canton && d.Id == code.District,
            cancellationToken);
        return row is null ? null : ToDistrict(row);
    }

    public async Task<IReadOnlyList<District>> GetDistrictsInAnyProvinceAsync(int canton, CancellationToken cancellationToken) =>
        // Primary-key order, which is what the original query returned without an ORDER BY.
        ToDistricts(await Districts
            .Where(d => d.CantonId == canton)
            .OrderBy(d => d.Id).ThenBy(d => d.CantonId).ThenBy(d => d.CantonProvinceId)
            .ToListAsync(cancellationToken));

    public async Task<District?> FindDistrictInAnyProvinceAsync(int canton, int district, CancellationToken cancellationToken)
    {
        var row = await Districts
            .Where(d => d.CantonId == canton && d.Id == district)
            .OrderBy(d => d.CantonProvinceId)
            .FirstOrDefaultAsync(cancellationToken);
        return row is null ? null : ToDistrict(row);
    }

    public async Task<IReadOnlyList<Province>> GetProvincesByCodeAsync(IReadOnlyCollection<ProvinceCode> codes, CancellationToken cancellationToken)
    {
        var ids = codes.Select(c => c.Value).Distinct().ToList();
        return ToProvinces(await Provinces.Where(p => ids.Contains(p.Id)).ToListAsync(cancellationToken));
    }

    public async Task<IReadOnlyList<Canton>> GetCantonsByCodeAsync(IReadOnlyCollection<CantonCode> codes, CancellationToken cancellationToken)
    {
        // SQL has no composite IN: fetch the cross product of the parts, then keep the exact pairs.
        var provinceIds = codes.Select(c => c.Province.Value).Distinct().ToList();
        var cantonIds = codes.Select(c => c.Canton).Distinct().ToList();
        var wanted = codes.ToHashSet();

        var rows = await Cantons
            .Where(c => provinceIds.Contains(c.ProvinceId) && cantonIds.Contains(c.Id))
            .ToListAsync(cancellationToken);

        return [.. ToCantons(rows).Where(c => wanted.Contains(c.Code))];
    }

    public async Task<IReadOnlyList<District>> GetDistrictsByCodeAsync(IReadOnlyCollection<DistrictCode> codes, CancellationToken cancellationToken)
    {
        var provinceIds = codes.Select(c => c.Canton.Province.Value).Distinct().ToList();
        var cantonIds = codes.Select(c => c.Canton.Canton).Distinct().ToList();
        var districtIds = codes.Select(c => c.District).Distinct().ToList();
        var wanted = codes.ToHashSet();

        var rows = await Districts
            .Where(d => provinceIds.Contains(d.CantonProvinceId) && cantonIds.Contains(d.CantonId) && districtIds.Contains(d.Id))
            .ToListAsync(cancellationToken);

        return [.. ToDistricts(rows).Where(d => wanted.Contains(d.Code))];
    }

    private static Province ToProvince(ProvinceRow row) => Province.Create(ProvinceCode.Create(row.Id), row.Name);

    private static Canton ToCanton(CantonRow row) =>
        Canton.Create(CantonCode.Create(ProvinceCode.Create(row.ProvinceId), row.Id), row.Name);

    private static District ToDistrict(DistrictRow row) =>
        District.Create(
            DistrictCode.Create(CantonCode.Create(ProvinceCode.Create(row.CantonProvinceId), row.CantonId), row.Id),
            row.Name);

    private static List<Province> ToProvinces(IEnumerable<ProvinceRow> rows) => [.. rows.Select(ToProvince)];

    private static List<Canton> ToCantons(IEnumerable<CantonRow> rows) => [.. rows.Select(ToCanton)];

    private static List<District> ToDistricts(IEnumerable<DistrictRow> rows) => [.. rows.Select(ToDistrict)];
}
