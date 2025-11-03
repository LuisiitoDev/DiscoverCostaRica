using DiscoverCostaRica.Function.LogConsumer.Models;
using Microsoft.Extensions.Logging;

namespace DiscoverCostaRica.Function.LogConsumer.Interfaces;

public interface IMongoLogger : ILogger
{
    void Log(LogEntryModel log);
}
