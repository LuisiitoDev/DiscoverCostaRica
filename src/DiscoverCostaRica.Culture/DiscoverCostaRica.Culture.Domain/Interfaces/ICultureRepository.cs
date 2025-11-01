using DiscoverCostaRica.Culture.Domain.Models;

namespace DiscoverCostaRica.Culture.Domain.Interfaces;

public interface ICultureRepository
{
    Task<List<DishModel>> GetDishes(CancellationToken cancellationToken);
    Task<List<TraditionModel>> GetTraditions(CancellationToken cancellationToken);
}
