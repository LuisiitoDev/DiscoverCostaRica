
using DiscoverCostaRica.Api.Models.Dto;
using DiscoverCostaRica.Api.Routes;
using DiscoverCostaRica.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverCostaRica.Api.Endpoints;

public class Country : IEndpoint
{
    public static void Register(WebApplication app)
    {
        app.MapGet(EndpointRoutes.COUNTRY_ROUTES.GET_COUNTRY, GetCountry)
            .WithName("GetCountry")
            .WithOpenApi(OpenApiCountry.GetCountry)
            .Produces<DtoCountry>(StatusCodes.Status200OK);

    }

    private static async Task<IResult> GetCountry([FromServices] CountryService service, CancellationToken cancellationToken, [FromQuery] int countryCode = 506)
    {
        var country = await service.GetCountry(countryCode, cancellationToken);
        if (country.IsNotFound) return Results.NotFound("No country was found.");
        if (country.IsSuccess) return Results.Ok(country.Value);
        return Results.BadRequest("It occurred an error, try later.");
    }
}
