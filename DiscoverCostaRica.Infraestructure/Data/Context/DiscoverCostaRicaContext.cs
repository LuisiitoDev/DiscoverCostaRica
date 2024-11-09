namespace DiscoverCostaRica.Infraestructure.Data.Context;

using DiscoverCostaRica.Domain.Entities;
using DiscoverCostaRica.Domain.Entities.Direction;
using DiscoverCostaRica.Infraestructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
public class DiscoverCostaRicaContext : DbContext
{
    public DbSet<Beach> Beaches { get; set; }
    public DbSet<Province> Provinces { get; set; }
    public DbSet<Canton> Cantons { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<ProvinceDetail> ProvinceDetails { get; set; }

    public DiscoverCostaRicaContext(DbContextOptions<DiscoverCostaRicaContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BeachConfiguration());
        modelBuilder.ApplyConfiguration(new ProvinceConfiguration());
        modelBuilder.ApplyConfiguration(new CantonConfiguration());
        modelBuilder.ApplyConfiguration(new DistrictConfiguration());
        modelBuilder.ApplyConfiguration(new ProvinceDetailConfiguration());
    }
}