using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Culture.Infrastructure.Context;

/// <summary>EF Core unit of work for the Culture tables. Public for migrations tooling; the rows it maps are internal.</summary>
public class CultureContext(DbContextOptions<CultureContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CultureContext).Assembly);
}
