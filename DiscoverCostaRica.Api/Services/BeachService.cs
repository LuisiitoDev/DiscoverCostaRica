using DiscoverCostaRica.Api.Models;
using DiscoverCostaRica.Domain.Entities;
using DiscoverCostaRica.Infraestructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Api.Services;

public class BeachService(DiscoverCostaRicaContext context)
{
	public async Task<Result<Beach[]>> GetBeaches(CancellationToken cancellationToken)
	{
		var beaches = await context.Beaches.ToArrayAsync(cancellationToken);
		
		if (beaches.Length > 0) return beaches;
		
		return Result<Beach[]>.NotFound("No beaches found. Please check later for updates.");
	}
}