using AutoMapper;
using DiscoverCostaRica.Culture.Application.Dtos;
using DiscoverCostaRica.Culture.Application.Interfaces;
using DiscoverCostaRica.Culture.Domain.Interfaces;
using DiscoverCostaRica.Shared.Responses;
using Microsoft.AspNetCore.Http;

namespace DiscoverCostaRica.Culture.Application.Services;

public class CultureService(ICultureRepository _repository, IMapper _mapper) : ICultureService
{
    public async Task<Result<List<DtoDish>>> GetDishes(CancellationToken cancellationToken)
    {
        var dishes = await _repository.GetDishes(cancellationToken);

        if (dishes is null || dishes.Count <= 0)
            return new Failure("No dishes found. Please check later for updates.", StatusCodes.Status404NotFound);

        var results = _mapper.Map<List<DtoDish>>(dishes);
        return new Success(results);
    }

    public async Task<Result<List<DtoTradition>>> GetTraditions(CancellationToken cancellationToken)
    {
        var traditions = await _repository.GetTraditions(cancellationToken);

        if (traditions is null || traditions.Count <= 0)
            return new Failure("No traditions found. Please check later for updates.", StatusCodes.Status404NotFound);

        var results = _mapper.Map<List<DtoTradition>>(traditions);
        return new Success(results);
    }
}
