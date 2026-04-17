using DiscoverCostaRica.VolcanoService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.VolcanoService.Infrastructure.Interfaces;

public interface IVolcanoContext
{
    public DbSet<VolcanoModel> Volcanos { get; set; }
}
