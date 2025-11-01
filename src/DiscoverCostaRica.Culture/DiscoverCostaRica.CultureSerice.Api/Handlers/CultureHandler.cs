using DiscoverCostaRica.Culture.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverCostaRica.Culture.Api.Handlers;

public static class CultureHandler
{
    public static class DishHandler
    {
        public static async Task<IResult> GetDishes([FromServices] ICultureService service, CancellationToken cancellationToken)
        {
            var result = await service.GetDishes(cancellationToken);
            return result.ToResult();
        }
    }

    public static class TraditionHandler
    {
        public static async Task<IResult> GetTraditions([FromServices] ICultureService service, CancellationToken cancellationToken)
        {
            var result = await service.GetTraditions(cancellationToken);
            return result.ToResult();
        }
    }
}
