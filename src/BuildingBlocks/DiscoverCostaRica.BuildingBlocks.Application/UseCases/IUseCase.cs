using DiscoverCostaRica.BuildingBlocks.Application.Results;

namespace DiscoverCostaRica.BuildingBlocks.Application.UseCases;

/// <summary>
/// Input boundary of one application behaviour (Clean Architecture "interactor").
/// Controllers depend on this; decorators (caching, logging) wrap it.
/// </summary>
public interface IUseCase<in TRequest, TResponse>
{
    Task<Result<TResponse>> ExecuteAsync(TRequest request, CancellationToken cancellationToken);
}
