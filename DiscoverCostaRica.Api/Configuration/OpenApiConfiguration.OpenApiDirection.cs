using Microsoft.OpenApi.Models;

namespace DiscoverCostaRica.Api.Configuration;

public static partial class OpenApiConfiguration
{
    public static class OpenApiDirection
    {
        public static OpenApiOperation GetProvince(OpenApiOperation operation)
        {
            operation.Summary = "";
            operation.Description = "";
            return operation;
        }

        public static OpenApiOperation GetCantons(OpenApiOperation operation)
        {
            operation.Summary = "";
            operation.Description = "";
            return operation;
        }

        public static OpenApiOperation GetDistricts(OpenApiOperation operation)
        {
            operation.Summary = "";
            operation.Description = "";
            return operation;
        }
    }
}
