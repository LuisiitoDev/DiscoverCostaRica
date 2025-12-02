using Azure.Core;
using Azure.Identity;
using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Shared.Authentication;
using DiscoverCostaRica.Shared.Interfaces;
using Microsoft.Extensions.Options;

namespace DiscoverCostaRica.Shared.Services;

[TransientService]
public class DiscoverCostaRicaTokenAcquisitionService(IOptionsMonitor<DiscoverCostaRicaTokenOptions> options) : IDiscoverCostaRicaTokenAcquisitionService
{
    private readonly TokenCredential _credential = new ClientSecretCredential(
        options.CurrentValue.TenantId,
        options.CurrentValue.ClientId,
        options.CurrentValue.ClientSecret);

    public async Task<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        var result = await _credential.GetTokenAsync(new TokenRequestContext([options.CurrentValue.Scope]), cancellationToken);
        return result.Token;
    }
}
