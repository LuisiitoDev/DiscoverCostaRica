using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DiscoverCostaRica.Infraestructure.Data.Context;
using DiscoverCostaRica.Functions.Services;

namespace DiscoverCostaRica.Functions
{
    public class CrawlerBeaches(CrawlerBeachService crawler, DiscoverCostaRicaContext context, ILogger<CrawlerBeachService> logger)
    {
        [Function("CrawlerBeaches")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            var beaches = await crawler.FetchBeachesAsync();
            await context.Beaches.AddRangeAsync(beaches);
            await context.SaveChangesAsync();

            return new OkObjectResult("Beaches loaded!");
        }
    }
}
