using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.VolcanoService.Domain.Interfaces;
using DiscoverCostaRica.VolcanoService.Domain.Models;
using DiscoverCostaRica.VolcanoService.Infraestructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.VolcanoService.Infraestructure.Services;

[TransientService]
public class VolcanoRepository(IVolcanoContext _context) : IVolcanoRepository
{
    public async Task<List<VolcanoModel>> GetVolcanos(CancellationToken cancellationToken)
    {
        return await _context.Volcanos.ToListAsync(cancellationToken);
    }

    public async Task<VolcanoModel?> GetVolcanoById(int id, CancellationToken cancellationToken)
    {
        return await _context.Volcanos.FirstOrDefaultAsync(volcano => volcano.Id == id, cancellationToken);
    }

    public async Task<List<VolcanoModel>> GetVolcanosByProvince(int provinceId, CancellationToken cancellationToken)
    {
        return await _context.Volcanos.Where(volcano => volcano.ProvinceId == provinceId).ToListAsync(cancellationToken);
    }
}
