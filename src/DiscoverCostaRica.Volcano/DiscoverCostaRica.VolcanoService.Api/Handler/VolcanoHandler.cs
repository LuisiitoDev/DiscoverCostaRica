using DiscoverCostaRica.VolcanoService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverCostaRica.VolcanoService.Api.Handler;

public static class VolcanoHandler
{
    public static async Task<IResult> GetVolcanos([FromServices] IVolcanoService service, CancellationToken cancellationToken)
    {
        var result = await service.GetVolcanos(cancellationToken);
        return result.ToResult();
    }

    public static async Task<IResult> GetVolcanoById([FromQuery] int id, [FromServices] IVolcanoService service, CancellationToken cancellationToken)
    {
        var result = await service.GetVolcanoById(id, cancellationToken);
        return result.ToResult();
    }

    public static async Task<IResult> GetVolcanosByProvince([FromQuery] int id, [FromServices] IVolcanoService service, CancellationToken cancellationToken)
    {
        var result = await service.GetVolcanosByProvince(id, cancellationToken);
        return result.ToResult();
    }
}
