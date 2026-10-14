using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Geo.Infrastructure.Context;

/// <summary>EF Core unit of work for the Geo tables. Public for migrations tooling; the rows it maps are internal.</summary>
public class GeoContext(DbContextOptions<GeoContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GeoContext).Assembly);
}
