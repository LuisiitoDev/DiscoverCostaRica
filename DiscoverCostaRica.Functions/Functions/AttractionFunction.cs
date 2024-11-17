using System;
using DiscoverCostaRica.Functions.Services;
using DiscoverCostaRica.Functions.Services.Azure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DiscoverCostaRica.Functions.Functions;

public class AttractionFunction
{
    private readonly ILogger _logger;

    public AttractionFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AttractionFunction>();
    }

    [Function("AttractionFunction")]
    public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

        var crawlerResult = VolcanoService.StartCrawler();

        await EventGridService.PublishEventToEventGrid(crawlerResult, _logger);
    }
}
