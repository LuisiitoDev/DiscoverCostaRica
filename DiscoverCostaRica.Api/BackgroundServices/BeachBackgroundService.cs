
using DiscoverCostaRica.Api.Services.Crawler;
using DiscoverCostaRica.Infraestructure.Data.Context;

namespace DiscoverCostaRica.Api.BackgroundServices;

public class BeachBackgroundService(
	ILogger<BeachBackgroundService> logger,
	BeachCrawlerService crawlerService,
	DiscoverCostaRicaContext context
	) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			logger.LogInformation("Beach background service running at: {time}", DateTimeOffset.Now);
			var beaches = await crawlerService.FetchBeachesAsync(stoppingToken);

			await context!.Beaches.AddRangeAsync(beaches, stoppingToken);
			await context.SaveChangesAsync(stoppingToken);
			
			await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
		}
	}
}