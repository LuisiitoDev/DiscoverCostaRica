using AutoMapper;
using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Shared.Responses;
using DiscoverCostaRica.VolcanoService.Application.Dtos;
using DiscoverCostaRica.VolcanoService.Application.Interfaces;
using DiscoverCostaRica.VolcanoService.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DiscoverCostaRica.VolcanoService.Application.Services;

[TransientService]
public class VolcanoService(IVolcanoRepository _repository, IMapper _mapper) : IVolcanoService
{
    public async Task<Result<VolcanoDto>> GetVolcanoById(int id, CancellationToken cancellationToken)
    {
        var volcano = await _repository.GetVolcanoById(id, cancellationToken);

        if (volcano is null)
            return new Failure("Volcano was not found", StatusCodes.Status404NotFound);

        return new Success(_mapper.Map<VolcanoDto>(volcano));
    }

    public async Task<Result<List<VolcanoDto>>> GetVolcanos(CancellationToken cancellationToken)
    {
        var volcanos = await _repository.GetVolcanos(cancellationToken);

        if (volcanos is null || volcanos.Count <= 0)
            return new Failure("Volcanos were not found", StatusCodes.Status404NotFound);

        return new Success(_mapper.Map<List<VolcanoDto>>(volcanos));
    }

    public async Task<Result<List<VolcanoDto>>> GetVolcanosByProvince(int provinceId, CancellationToken cancellationToken)
    {
        var volcanos = await _repository.GetVolcanosByProvince(provinceId, cancellationToken);

        if (volcanos is null || volcanos.Count <= 0)
            return new Failure("Volcanos were not found", StatusCodes.Status404NotFound);

        return new Success(_mapper.Map<List<VolcanoDto>>(volcanos));
    }
}
