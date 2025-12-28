using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Shared.Responses;
using DiscoverCostaRica.Volcano.Application.Dtos;
using DiscoverCostaRica.Volcano.Application.Interfaces;
using DiscoverCostaRica.VolcanoService.Application.Interfaces;
using DiscoverCostaRica.VolcanoService.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DiscoverCostaRica.Volcano.Application.Services;

/// <summary>
/// Service for volcano-related operations.
/// </summary>
[TransientService]
public class VolcanoService(IVolcanoRepository repository, ILocationService geo, IMapperVolcanoService mapper) : IVolcanoService
{
    /// <summary>
    /// Retrieves a volcano by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the volcano.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A <see cref="Result{DtoVolcano}"/> containing the volcano data if found; otherwise, a failure result.
    /// </returns>
    public async Task<Result<DtoVolcano>> GetVolcanoById(int id, CancellationToken cancellationToken)
    {
        var volcano = await repository.GetVolcanoById(id, cancellationToken);

        if (volcano is null)
            return new Failure("Volcano was not found", StatusCodes.Status404NotFound);

        return new Success(mapper.MapVolcano(volcano));
    }

    /// <summary>
    /// Retrieves all volcanos with their locations.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A <see cref="Result{List{VolcanoDto}}"/> containing a list of volcanos with location data if found; otherwise, a failure result.
    /// </returns>
    public async Task<Result<IEnumerable<DtoVolcano>>> GetVolcanos(CancellationToken cancellationToken)
    {
        var volcanosModel = await repository.GetVolcanos(cancellationToken);

        if (volcanosModel is null || volcanosModel.Count <= 0)
            return new Failure("Volcanos were not found", StatusCodes.Status404NotFound);

        var volcanoLocations = await geo.GetVolcanoLocations(volcanosModel, cancellationToken);

        return new Success(mapper.MapVolcanoesWithLocations(volcanosModel, volcanoLocations));
    }

    /// <summary>
    /// Retrieves volcanos by province.
    /// </summary>
    /// <param name="provinceId">The unique identifier of the province.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A <see cref="Result{List{DtoVolcano}}"/> containing a list of volcanos in the specified province if found; otherwise, a failure result.
    /// </returns>
    public async Task<Result<List<DtoVolcano>>> GetVolcanosByProvince(int provinceId, CancellationToken cancellationToken)
    {
        var volcanos = await repository.GetVolcanosByProvince(provinceId, cancellationToken);

        if (volcanos is null || volcanos.Count <= 0)
            return new Failure("Volcanos were not found", StatusCodes.Status404NotFound);

        return new Success(mapper.MapVolcanos(volcanos));
    }
}
