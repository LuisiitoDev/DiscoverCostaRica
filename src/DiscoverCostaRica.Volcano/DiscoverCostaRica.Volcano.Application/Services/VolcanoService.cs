using AutoMapper;
using Dapr.Client;
using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Shared.Constants;
using DiscoverCostaRica.Shared.Responses;
using DiscoverCostaRica.VolcanoService.Application.Dtos;
using DiscoverCostaRica.VolcanoService.Application.Interfaces;
using DiscoverCostaRica.VolcanoService.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DiscoverCostaRica.VolcanoService.Application.Services;

[TransientService]
public class VolcanoService(IVolcanoRepository repository, IMapper mapper, DaprClient dapr, ILogger<VolcanoService> logger) : IVolcanoService
{
    public async Task<Result<VolcanoDto>> GetVolcanoById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var volcano = await repository.GetVolcanoById(id, cancellationToken);

            if (volcano is null)
                return new Failure("Volcano was not found", StatusCodes.Status404NotFound);

            return new Success(mapper.Map<VolcanoDto>(volcano));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandle exception");
            return new Failure("Unhandle exception", StatusCodes.Status500InternalServerError);
        }

    }

    public async Task<Result<List<VolcanoDto>>> GetVolcanos(CancellationToken cancellationToken)
    {
        try
        {
            var volcanosModel = await repository.GetVolcanos(cancellationToken);

            if (volcanosModel is null || volcanosModel.Count <= 0)
                return new Failure("Volcanos were not found", StatusCodes.Status404NotFound);

            var volcanos = mapper.Map<List<VolcanoDto>>(volcanosModel);

            foreach (var volcano in volcanosModel)
            {
                var province = await dapr.InvokeMethodAsync<object>(
                    HttpMethod.Get,
                    Microservices.Geo,
                    $"api/provinces/{volcano.ProvinceId}",
                    cancellationToken
                );

                var canton = await dapr.InvokeMethodAsync<object>(
                    HttpMethod.Get,
                    Microservices.Geo,
                    $"api/cantons/{volcano.CantonId}",
                    cancellationToken
                );

                if (volcano.DistrictId.HasValue)
                {
                    var district = await dapr.InvokeMethodAsync<object>(
                        HttpMethod.Get,
                        Microservices.Geo,
                        $"api/districts/{volcano.DistrictId.Value}",
                        cancellationToken
                    );
                }
            }

            return new Success(volcanos);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandle exception");
            return new Failure("Unhandle exception", StatusCodes.Status500InternalServerError);
        }

    }

    public async Task<Result<List<VolcanoDto>>> GetVolcanosByProvince(int provinceId, CancellationToken cancellationToken)
    {
        try
        {
            var volcanos = await repository.GetVolcanosByProvince(provinceId, cancellationToken);

            if (volcanos is null || volcanos.Count <= 0)
                return new Failure("Volcanos were not found", StatusCodes.Status404NotFound);

            return new Success(mapper.Map<List<VolcanoDto>>(volcanos));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandle exception");
            return new Failure("Unhandle exception", StatusCodes.Status500InternalServerError);
        }
    }
}
