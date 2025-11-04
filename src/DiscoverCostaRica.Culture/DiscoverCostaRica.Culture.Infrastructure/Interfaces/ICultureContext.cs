using DiscoverCostaRica.Culture.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Culture.Infraestructure.Interfaces;

public interface ICultureContext
{
    public DbSet<DishModel> Dishes { get; set; }
    public DbSet<TraditionModel> Traditions { get; set; }
}
