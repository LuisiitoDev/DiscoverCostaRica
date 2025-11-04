using DiscoverCostaRica.Geo.Domain.Interfaces;
using DiscoverCostaRica.Geo.Domain.Models;
using DiscoverCostaRica.Geo.Infraestructure.Interfaces;
using DiscoverCostaRica.Shared.Attributes;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Geo.Infraestructure.Services;

[TransientService]
public class GeoRepository(IGeoContext _context) : IGeoRepository
{
    public async Task<List<CantonModel>> GetCantonsByProvince(int provinceId, CancellationToken cancellationToken)
    {
        return await _context.Cantons.Where(canton => canton.ProvinceId == provinceId).ToListAsync(cancellationToken);
    }

    public async Task<List<DistrictModel>> GetDistrictsByCanton(int cantonId, CancellationToken cancellationToken)
    {
        return await _context.Districts.Where(district => district.CantonId == cantonId).ToListAsync(cancellationToken);
    }

    public async Task<List<ProvinceModel>> GetProvinces(CancellationToken cancellationToken)
    {
        return await _context.Provinces.ToListAsync(cancellationToken);
    }
}
