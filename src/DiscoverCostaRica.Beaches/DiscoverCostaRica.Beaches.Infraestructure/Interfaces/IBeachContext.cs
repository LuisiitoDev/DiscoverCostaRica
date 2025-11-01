using DiscoverCostaRica.Beaches.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Beaches.Infraestructure.Interfaces;

public interface IBeachContext
{
    public DbSet<BeachModel> Beaches { get; set; }
}
