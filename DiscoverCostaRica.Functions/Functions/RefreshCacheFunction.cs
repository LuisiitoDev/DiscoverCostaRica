using DiscoverCostaRica.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DiscoverCostaRica.Functions.Functions
{
    public class RefreshCacheFunction
    {
        private readonly ILogger _logger;
        private readonly VolcanoService volcanoService;
        private readonly BeachService beachService;
        private readonly DishService dishService;
        private readonly ProvinceService provinceService;

        public RefreshCacheFunction(ILoggerFactory loggerFactory,
            VolcanoService volcanoService,
            BeachService beachService,
            DishService dishService,
            ProvinceService provinceService)
        {
            _logger = loggerFactory.CreateLogger<RefreshCacheFunction>();
            this.volcanoService = volcanoService;
            this.beachService = beachService;
            this.dishService = dishService;
            this.provinceService = provinceService;
        }

        [Function("RefreshCacheFunction")]
        public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            await volcanoService.Sync();
            await beachService.Sync();
            await dishService.Sync();
            await provinceService.Sync();

        }
    }
}
