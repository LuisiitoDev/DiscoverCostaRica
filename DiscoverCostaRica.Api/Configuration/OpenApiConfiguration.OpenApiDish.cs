using Microsoft.OpenApi.Models;

namespace DiscoverCostaRica.Api.Configuration;

public static partial class OpenApiConfiguration
{
    public static class OpenApiDish
    {
        public static OpenApiOperation GetDish(OpenApiOperation operation)
        {
            operation.Summary = "";
            operation.Description = "";
            return operation;
        }
    }
}
