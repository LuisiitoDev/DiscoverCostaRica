namespace DiscoverCostaRica.Shared.Interfaces;

public interface IDiscoverCostaRicaDapr
{
    Task<TResult?> InvokeGetMethodAsync<TResult>(string appId, string endpoint, CancellationToken cancellationToken);
}
