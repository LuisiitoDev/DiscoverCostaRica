using DiscoverCostaRica.Geo.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Geo.Infrastructure.Interfaces;

public interface IGeoContext
{
    public DbSet<ProvinceModel> Provinces { get; set; }
    public DbSet<CantonModel> Cantons { get; set; }
    public DbSet<DistrictModel> Districts { get; set; }
}
