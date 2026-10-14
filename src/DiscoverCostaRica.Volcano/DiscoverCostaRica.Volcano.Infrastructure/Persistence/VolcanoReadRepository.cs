using DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection;
using DiscoverCostaRica.Volcano.Application.Ports;
using DiscoverCostaRica.Volcano.Domain;
using DiscoverCostaRica.Volcano.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Volcano.Infrastructure.Persistence;

[TransientService]
internal sealed class VolcanoReadRepository(VolcanoContext context) : IVolcanoReadRepository
{
    private IQueryable<VolcanoRow> Volcanoes => context.Set<VolcanoRow>().AsNoTracking();

    public async Task<IReadOnlyList<Domain.Volcano>> GetAllAsync(CancellationToken cancellationToken) =>
        [.. (await Volcanoes.OrderBy(v => v.Id).ToListAsync(cancellationToken)).Select(ToVolcano)];

    public async Task<Domain.Volcano?> FindAsync(int id, CancellationToken cancellationToken)
    {
        var row = await Volcanoes.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
        return row is null ? null : ToVolcano(row);
    }

    public async Task<IReadOnlyList<Domain.Volcano>> GetByProvinceAsync(int provinceId, CancellationToken cancellationToken) =>
        [.. (await Volcanoes.Where(v => v.ProvinceId == provinceId).OrderBy(v => v.Id).ToListAsync(cancellationToken)).Select(ToVolcano)];

    private static Domain.Volcano ToVolcano(VolcanoRow row) =>
        Domain.Volcano.Create(row.Id, row.Name, row.Description, LocationCode.Create(row.ProvinceId, row.CantonId, row.DistrictId));
}
