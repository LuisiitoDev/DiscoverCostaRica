using DiscoverCostaRica.Geo.Domain.Interfaces;
using DiscoverCostaRica.Geo.Domain.Models;
using DiscoverCostaRica.Geo.Infraestructure.Interfaces;
using DiscoverCostaRica.Shared.Attributes;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Geo.Infraestructure.Services;

[TransientService]
/// <summary>
/// Repository implementing geographic data access for provinces, cantons and districts.
/// Uses an <see cref="IGeoContext"/> to query the underlying data store.
/// </summary>
/// <param name="_context">The geo data context used to access EF Core DbSets.</param>
public class GeoRepository(IGeoContext _context) : IGeoRepository
{
    /// <summary>
    /// Retrieves a province by its identifier.
    /// </summary>
    /// <param name="provinceId">The identifier of the province to retrieve.</param>
    /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A <see cref="ProvinceModel"/> instance when a province with the specified identifier exists; otherwise <c>null</c>.
    /// </returns>
    public async Task<ProvinceModel?> GetProvinceById(int provinceId, CancellationToken cancellationToken)
    {
        return await _context.Provinces.FindAsync([provinceId, cancellationToken], cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Retrieves a canton by its province and canton identifiers.
    /// </summary>
    /// <param name="provinceId">The identifier of the province that contains the canton.</param>
    /// <param name="cantonId">The identifier of the canton to retrieve.</param>
    /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A <see cref="CantonModel"/> instance when a canton with the specified identifiers exists; otherwise <c>null</c>.
    /// </returns>
    public async Task<CantonModel?> GetCantonById(int provinceId, int cantonId, CancellationToken cancellationToken)
    {
        return await _context.Cantons.FirstOrDefaultAsync(c => c.ProvinceId == provinceId && c.Id == cantonId, cancellationToken);
    }

    /// <summary>
    /// Retrieves a district by its canton and district identifiers.
    /// </summary>
    /// <param name="cantonId">The identifier of the canton that contains the district.</param>
    /// <param name="districtId">The identifier of the district to retrieve.</param>
    /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A <see cref="DistrictModel"/> instance when a district with the specified identifiers exists; otherwise <c>null</c>.
    /// </returns>
    public async Task<DistrictModel?> GetDistrictById(int cantonId, int districtId, CancellationToken cancellationToken)
    {
        return await _context.Districts.FirstOrDefaultAsync(d => d.CantonId == cantonId && d.Id == districtId, cancellationToken);
    }

    /// <summary>
    /// Retrieves all cantons that belong to the specified province.
    /// </summary>
    /// <param name="provinceId">The identifier of the province whose cantons will be returned.</param>
    /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A list of <see cref="CantonModel"/> instances that belong to the specified province. The list will be empty if none are found.
    /// </returns>
    public async Task<List<CantonModel>> GetCantonsByProvince(int provinceId, CancellationToken cancellationToken)
    {
        return await _context.Cantons.Where(canton => canton.ProvinceId == provinceId).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves all districts that belong to the specified canton.
    /// </summary>
    /// <param name="cantonId">The identifier of the canton whose districts will be returned.</param>
    /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A list of <see cref="DistrictModel"/> instances that belong to the specified canton. The list will be empty if none are found.
    /// </returns>
    public async Task<List<DistrictModel>> GetDistrictsByCanton(int cantonId, CancellationToken cancellationToken)
    {
        return await _context.Districts.Where(district => district.CantonId == cantonId).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves all provinces.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A list containing all <see cref="ProvinceModel"/> instances. The list will be empty if no provinces exist.
    /// </returns>
    public async Task<List<ProvinceModel>> GetProvinces(CancellationToken cancellationToken)
    {
        return await _context.Provinces.ToListAsync(cancellationToken);
    }
}
