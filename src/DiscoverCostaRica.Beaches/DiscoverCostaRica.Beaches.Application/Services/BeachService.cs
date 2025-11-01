using AutoMapper;
using DiscoverCostaRica.Beaches.Application.Dtos;
using DiscoverCostaRica.Beaches.Application.Interfaces;
using DiscoverCostaRica.Beaches.Domain.Interfaces;
using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Shared.Responses;
using Microsoft.AspNetCore.Http;
namespace DiscoverCostaRica.Beaches.Application.Services;

[TransientService]
public class BeachService(IBeachRepository _repository, IMapper _mapper) : IBeachService
{
    public async Task<Result<List<DtoBeach>>> GetBeaches(CancellationToken cancellationToken)
    {
        var beaches = await _repository.GetBeaches(cancellationToken);

        if (beaches is null || beaches.Count <= 0)
            return new Failure("No beaches found. Please check later for updates.", StatusCodes.Status404NotFound);

        var results = _mapper.Map<List<DtoBeach>>(beaches);
        return new Success(results);
    }
}
