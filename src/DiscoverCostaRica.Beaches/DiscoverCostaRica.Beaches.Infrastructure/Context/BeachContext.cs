using DiscoverCostaRica.Beaches.Domain.Models;
using DiscoverCostaRica.Beaches.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Beaches.Infrastructure.Context;

public class BeachContext : DbContext, IBeachContext
{
    public BeachContext(DbContextOptions<BeachContext> options) : base(options) { }

    public DbSet<BeachModel> Beaches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BeachContext).Assembly);
    }
}
