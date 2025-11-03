using AutoMapper;
using DiscoverCostaRica.Beaches.Application.Dtos;
using DiscoverCostaRica.Beaches.Application.Interfaces;
using DiscoverCostaRica.Beaches.Domain.Interfaces;
using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
namespace DiscoverCostaRica.Beaches.Application.Services;

[TransientService]
public class BeachService(IBeachRepository repository, IMapper mapper, ILogger<BeachService> logger) : IBeachService
{
    public async Task<Result<List<DtoBeach>>> GetBeaches(CancellationToken cancellationToken)
    {
        try
        {
            var beaches = await repository.GetBeaches(cancellationToken);

            if (beaches is null || beaches.Count <= 0)
                return new Failure("No beaches found. Please check later for updates.", StatusCodes.Status404NotFound);

            var results = mapper.Map<List<DtoBeach>>(beaches);
            return new Success(results);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception ocurred");
            return new Failure("Unhandled exception ocurred", StatusCodes.Status500InternalServerError);
        }
    }
}
