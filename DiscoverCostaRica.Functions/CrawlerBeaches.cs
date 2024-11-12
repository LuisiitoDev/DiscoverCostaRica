using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverCostaRica.Functions
{
    public class CrawlerBeaches
    {
        private readonly ILogger<CrawlerBeaches> _logger;

        public CrawlerBeaches(ILogger<CrawlerBeaches> logger)
        {
            _logger = logger;
        }

        [Function("CrawlerBeaches")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
