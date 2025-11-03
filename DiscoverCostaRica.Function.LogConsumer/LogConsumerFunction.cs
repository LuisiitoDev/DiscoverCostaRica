// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using Azure.Messaging.EventGrid;
using DiscoverCostaRica.Function.LogConsumer.Interfaces;
using DiscoverCostaRica.Function.LogConsumer.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using System.Text.Json;

namespace DiscoverCostaRica.Function.LogConsumer
{
    public static class LogConsumerFunction
    {
        [FunctionName("LogConsumerFunction")]
        public static void Run([EventGridTrigger] EventGridEvent eventGridEvent, IMongoLogger log)
        {
            var content = JsonSerializer.Deserialize<LogEntryModel>(eventGridEvent.Data.ToArray());
            log.Log(content!);
        }
    }
}
