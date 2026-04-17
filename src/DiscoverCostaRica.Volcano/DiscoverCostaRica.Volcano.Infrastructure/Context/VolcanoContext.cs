using DiscoverCostaRica.VolcanoService.Domain.Models;
using DiscoverCostaRica.VolcanoService.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.VolcanoService.Infrastructure.Context;

public class VolcanoContext(DbContextOptions<VolcanoContext> options) : DbContext(options), IVolcanoContext
{
    public DbSet<VolcanoModel> Volcanos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VolcanoContext).Assembly);
    }
}
