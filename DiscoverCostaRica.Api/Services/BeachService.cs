using DiscoverCostaRica.Api.Constants;
using DiscoverCostaRica.Api.Models;
using DiscoverCostaRica.Domain.Entities;
using DiscoverCostaRica.Infraestructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DiscoverCostaRica.Api.Services;

public class BeachService(DiscoverCostaRicaContext context, RedisCacheService cache)
{
	public async Task<Result<Beach[]>> GetBeaches(CancellationToken cancellationToken)
	{
		if(await cache.ContainsKeyAsync(CacheKeys.ALL_BEACHES))
			return await cache.GetAsync<Beach[]>(CacheKeys.ALL_BEACHES);
		
		var beaches = await context.Beaches.ToArrayAsync(cancellationToken);
		
		if (beaches.Length > 0) 
		{
			await cache.SetAsync<Beach[]>(CacheKeys.ALL_BEACHES, beaches);
			return beaches;
		}
		
		return Result<Beach[]>.NotFound("No beaches found. Please check later for updates.");
	}
}