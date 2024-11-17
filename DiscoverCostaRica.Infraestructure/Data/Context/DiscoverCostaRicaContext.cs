namespace DiscoverCostaRica.Infraestructure.Data.Context;

using DiscoverCostaRica.Domain.Entities;
using DiscoverCostaRica.Infraestructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
public class DiscoverCostaRicaContext : DbContext
{
    public DiscoverCostaRicaContext(DbContextOptions<DiscoverCostaRicaContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public DbSet<Country> Countries { get; set; }
    public DbSet<Beach> Beaches { get; set; }
    public DbSet<Province> Provinces { get; set; }
    public DbSet<Canton> Cantons { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<Dish> Dishes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BeachConfiguration());
        modelBuilder.ApplyConfiguration(new ProvinceConfiguration());
        modelBuilder.ApplyConfiguration(new CantonConfiguration());
        modelBuilder.ApplyConfiguration(new DistrictConfiguration());
        modelBuilder.ApplyConfiguration(new DishConfiguration());
        modelBuilder.ApplyConfiguration(new AttractionConfiguration());
        modelBuilder.ApplyConfiguration(new CountryConfiguration());
    }
}