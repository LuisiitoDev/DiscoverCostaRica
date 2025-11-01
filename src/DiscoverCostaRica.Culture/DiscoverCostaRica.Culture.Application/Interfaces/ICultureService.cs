using DiscoverCostaRica.Culture.Application.Dtos;
using DiscoverCostaRica.Shared.Responses;

namespace DiscoverCostaRica.Culture.Application.Interfaces;

public interface ICultureService
{
    Task<Result<List<DtoDish>>> GetDishes(CancellationToken cancellationToken);
    Task<Result<List<DtoTradition>>> GetTraditions(CancellationToken cancellationToken);
}
