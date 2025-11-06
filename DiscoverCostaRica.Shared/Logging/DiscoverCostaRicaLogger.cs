using DiscoverCostaRica.Shared.Interfaces;
using Microsoft.Extensions.Logging;

namespace DiscoverCostaRica.Shared.Logging;

public class DiscoverCostaRicaLogger(ILoggerFunction logger, string categoryName) : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel == LogLevel.Error;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        logger.SendLogger(new LogEntryModel
        {
            Timestamp = DateTime.UtcNow,
            LogLevel = logLevel.ToString(),
            Category = categoryName,
            Message = formatter(state, exception),
            Exception = exception ?? new Exception("No exception provided")
        });
    }
}
