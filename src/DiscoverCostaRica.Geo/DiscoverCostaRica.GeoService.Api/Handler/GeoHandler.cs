using DiscoverCostaRica.Geo.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverCostaRica.Geo.Api.Handler;

public static class GeoHandler
{
    public static async Task<IResult> GetProvinces([FromServices] IGeoService service, CancellationToken cancellationToken)
    {
        var result = await service.GetProvinces(cancellationToken);
        return result.ToResult();
    }

    public static async Task<IResult> GetCantonsByProvince([FromQuery] int provinceId, [FromServices] IGeoService service, CancellationToken cancellationToken)
    {
        var result = await service.GetCantons(provinceId, cancellationToken);
        return result.ToResult();
    }

    public static async Task<IResult> GetDistrictsByCanton([FromQuery] int cantonId, [FromServices] IGeoService service, CancellationToken cancellationToken)
    {
        var result = await service.GetDistricts(cantonId, cancellationToken);
        return result.ToResult();
    }
}
