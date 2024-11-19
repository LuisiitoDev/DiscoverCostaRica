using Microsoft.OpenApi.Models;

namespace DiscoverCostaRica.Api.Configuration;

public static partial class OpenApiConfiguration
{
    public static class OpenApiCountry
    {
        public static OpenApiOperation GetCountry(OpenApiOperation operation)
        {
            operation.Summary = "Retrieve country information";
            operation.Description = """
            This endpoint retrieves detailed information about a specific country.
            It includes various data points such as the country's name, capital, population, area, 
            and other relevant geographical and political information.
            This endpoint is useful for applications that need to display or process country-specific data.
        """;
            return operation;
        }
    }
}
