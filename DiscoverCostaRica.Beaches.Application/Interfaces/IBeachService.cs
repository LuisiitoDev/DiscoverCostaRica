using DiscoverCostaRica.Beaches.Application.Dtos;
using DiscoverCostaRica.Shared.Responses;

namespace DiscoverCostaRica.Beaches.Application.Interfaces;

public interface IBeachService
{
    Task<Result<List<DtoBeach>>> GetBeaches(CancellationToken cancellationToken);
}
