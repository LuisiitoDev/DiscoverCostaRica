using Microsoft.OpenApi.Models;

namespace DiscoverCostaRica.Api.Configuration;

public static partial class OpenApiConfiguration
{
    public static class OpenApiBeach
    {
        public static OpenApiOperation GetBeach(OpenApiOperation operation)
        {
            operation.Summary = "Retrieve a list of beaches in Costa Rica";
            operation.Description = """
			This endpoint allows to retrieve a comprehensive list of beahes located in Costa Rica.
			You will receive details such as the name of the beach, location and other relevant information.
			Ideal for tourism, planning trips, or research purposes.
			""";
            return operation;
        }
    }
}
