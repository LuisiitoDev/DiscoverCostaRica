using DiscoverCostaRica.Beaches.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Beaches.Infrastructure.Interfaces;

public interface IBeachContext
{
    public DbSet<BeachModel> Beaches { get; set; }
}
