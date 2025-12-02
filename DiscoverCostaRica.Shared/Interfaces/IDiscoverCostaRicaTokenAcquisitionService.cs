namespace DiscoverCostaRica.Shared.Interfaces;

public interface IDiscoverCostaRicaTokenAcquisitionService
{
    Task<string> GetTokenAsync(CancellationToken cancellationToken);
}
