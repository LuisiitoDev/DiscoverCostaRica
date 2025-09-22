using DiscoverCostaRica.Beaches.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverCostaRica.Beaches.Api.Handlers;

public static class BeachHandler
{
    public static async Task<IResult> ExecuteAsync([FromServices] IBeachService service, CancellationToken cancellationToken)
    {
        var result = await service.GetBeaches(cancellationToken);
        return result.ToResult();
    }
}
