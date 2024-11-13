
using DiscoverCostaRica.Api.Routes;
using DiscoverCostaRica.Api.Services;

namespace DiscoverCostaRica.Api.Endpoints;

public class Direction : IEndpoint
{
	public static void Register(WebApplication app)
	{
		app.MapGet(EndpointRoutes.DIRECTIONROUTES.GET_PROVINCES, GetProvinces);
		app.MapGet(EndpointRoutes.DIRECTIONROUTES.GET_CANTONS, GetCantons);
		app.MapGet(EndpointRoutes.DIRECTIONROUTES.GET_DISTRICTS, GetDistricts);
	}

	private static async Task<IResult> GetProvinces(DirectionService service, CancellationToken cancellationToken)
	{
		var provinces = await service.GetProvinces(cancellationToken);
		if (provinces.IsSuccess) return Results.Ok(provinces.Value);
		if (provinces.IsNotFound) return Results.NotFound(provinces.Error);
		return Results.BadRequest(provinces.Error);
	}
	private static async Task<IResult> GetCantons(DirectionService service, int provinceId, CancellationToken cancellationToken)
	{
		var cantons = await service.GetCantons(provinceId, cancellationToken);
		if (cantons.IsSuccess) return Results.Ok(cantons.Value);
		if (cantons.IsNotFound) return Results.NotFound(cantons.Error);
		return Results.BadRequest(cantons.Error);
	}
	private static async Task<IResult> GetDistricts(DirectionService service, int provinceId, int cantonId, CancellationToken cancellationToken)
	{
		var districts = await service.GetDistricts(provinceId, cantonId, cancellationToken);
		if (districts.IsSuccess) return Results.Ok(districts.Value);
		if (districts.IsNotFound) return Results.NotFound(districts.Error);
		return Results.BadRequest(districts.Error);
	}
}