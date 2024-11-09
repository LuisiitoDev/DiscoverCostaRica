
using DiscoverCostaRica.Api.Routes;
using DiscoverCostaRica.Api.Services;
using DiscoverCostaRica.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DiscoverCostaRica.Api.Endpoints;

public class Beach : IEndpoint
{
	public static void Register(WebApplication app)
	{
		app.MapGet(EndpointRoutes.BeachRoutes.GET_BEACHES, GetBeaches)
		.WithName("GetBeaches")
		.WithOpenApi(operation => new(operation)
		{
			Summary = "Retrieve a list of beaches in Costa Rica",
			Description = """
			This endpoint allows to retrieve a comprehensive list of beahes located in Costa Rica.
			You will receive details such as the name of the beach, location and other relevant information.
			Ideal for tourism, planning trips, or research purposes.
			"""
		})
		.Produces<Beach[]>(StatusCodes.Status200OK);
	}


	private static async Task<IResult> GetBeaches(BeachService service, CancellationToken cancellationToken)
	{
		var result = await service.GetBeaches(cancellationToken);
		if (result.IsSuccess) return Results.Ok(result.Value);
		if (result.IsNotFound) return Results.NotFound(result.Error);
		return Results.BadRequest(result.Error);
	}

}