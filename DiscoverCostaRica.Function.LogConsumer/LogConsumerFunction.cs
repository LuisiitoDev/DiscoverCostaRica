using DiscoverCostaRica.Function.LogConsumer.Interfaces;
using Microsoft.Azure.Functions.Worker;

namespace DiscoverCostaRica.Function.LogConsumer;

public class LogConsumerFunction(IQueueService _queue)
{

    [Function("LogConsumerFunction")]
    public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
    {
        _queue.ProcessQueue();
    }
}