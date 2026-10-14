using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Beaches.Infrastructure.Context;

/// <summary>EF Core unit of work for the Beach table. Public for migrations tooling; the row it maps is internal.</summary>
public class BeachContext(DbContextOptions<BeachContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BeachContext).Assembly);
}
