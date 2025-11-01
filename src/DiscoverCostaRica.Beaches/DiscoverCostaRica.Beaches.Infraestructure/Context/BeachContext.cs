using DiscoverCostaRica.Beaches.Domain.Models;
using DiscoverCostaRica.Beaches.Infraestructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Beaches.Infraestructure.Context;

public class BeachContext : DbContext, IBeachContext
{
    protected BeachContext(DbContextOptions<BeachContext> options) : base(options) { }

    public DbSet<BeachModel> Beaches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BeachContext).Assembly);
    }
}
