using DiscoverCostaRica.Culture.Domain.Models;
using DiscoverCostaRica.Culture.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Culture.Infrastructure.Context;

public class CultureContext(DbContextOptions<CultureContext> options) : DbContext(options), ICultureContext
{
    public DbSet<DishModel> Dishes { get; set; }
    public DbSet<TraditionModel> Traditions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CultureContext).Assembly);
    }
}
