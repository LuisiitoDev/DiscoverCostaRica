
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
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Retrieve country information",
                Description = """
            This endpoint retrieves detailed information about a specific country.
            It includes various data points such as the country's name, capital, population, area, 
            and other relevant geographical and political information.
            This endpoint is useful for applications that need to display or process country-specific data.
        """
            });

    }

    private static async Task<IResult> GetCountry([FromServices] CountryService service, CancellationToken cancellationToken, [FromQuery] int countryCode = 506)
    {
        var country = await service.GetCountry(countryCode, cancellationToken);
        if (country.IsNotFound) return Results.NotFound("No country was found.");
        if (country.IsSuccess) return Results.Ok(country.Value);
        return Results.BadRequest("It occurred an error, try later.");
    }
}
