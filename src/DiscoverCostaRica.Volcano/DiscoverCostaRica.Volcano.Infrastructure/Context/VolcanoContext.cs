using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Volcano.Infrastructure.Context;

/// <summary>EF Core unit of work for the Volcano table. Public for migrations tooling; the row it maps is internal.</summary>
public class VolcanoContext(DbContextOptions<VolcanoContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VolcanoContext).Assembly);
}
