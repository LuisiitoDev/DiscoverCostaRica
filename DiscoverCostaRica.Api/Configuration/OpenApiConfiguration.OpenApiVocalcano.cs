using Microsoft.OpenApi.Models;

namespace DiscoverCostaRica.Api.Configuration;

public static partial class OpenApiConfiguration
{
    public static class OpenApiVocalcano
    {
        public static OpenApiOperation GetVolcano(OpenApiOperation operation)
        {
            operation.Summary = "Get Costa Rica Volcanos";
            return operation;
        }
    }
}
