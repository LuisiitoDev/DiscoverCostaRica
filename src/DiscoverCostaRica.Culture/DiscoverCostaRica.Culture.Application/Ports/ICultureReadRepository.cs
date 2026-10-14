using DiscoverCostaRica.Culture.Domain;

namespace DiscoverCostaRica.Culture.Application.Ports;

public interface ICultureReadRepository
{
    /// <summary>Every tradition, ordered by id.</summary>
    Task<IReadOnlyList<Tradition>> GetTraditionsAsync(CancellationToken cancellationToken);

    /// <summary>Every typical dish, ordered by id.</summary>
    Task<IReadOnlyList<Dish>> GetDishesAsync(CancellationToken cancellationToken);
}
