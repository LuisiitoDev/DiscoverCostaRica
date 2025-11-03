using DiscoverCostaRica.Function.LogConsumer.Interfaces;
using DiscoverCostaRica.Function.LogConsumer.Models;
using Microsoft.Extensions.Logging;

namespace DiscoverCostaRica.Function.LogConsumer.Logging;

public class MongoLogger(IMongoService mongo) : IMongoLogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }

    public void Log(LogEntryModel log)
    {
        var logModel = new LogModel
        {
            Id = Guid.NewGuid().ToString(),
            Message = log.Message,
            Category = log.Category,
            Exception = log.Exception?.ToString() ?? string.Empty,
            Stacktrace = log.Exception?.StackTrace ?? string.Empty
        };

        _ = mongo.Log(logModel);
    }
}
