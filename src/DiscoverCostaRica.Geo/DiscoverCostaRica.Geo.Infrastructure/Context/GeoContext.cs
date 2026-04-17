using DiscoverCostaRica.Geo.Domain.Models;
using DiscoverCostaRica.Geo.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Geo.Infrastructure.Context;

public class GeoContext(DbContextOptions<GeoContext> options) : DbContext(options), IGeoContext
{
    public DbSet<ProvinceModel> Provinces { get; set; }
    public DbSet<CantonModel> Cantons { get; set; }
    public DbSet<DistrictModel> Districts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GeoContext).Assembly);
    }
}
