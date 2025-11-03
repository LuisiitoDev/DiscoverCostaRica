using Azure.Messaging.EventGrid;
using DiscoverCostaRica.Shared.EventGrid;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace DiscoverCostaRica.Shared.Logging;

public class EventGridLogger(IEventGridClient client, string categoryName) : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel == LogLevel.Error;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new LogEntryModel
        {
            Timestamp = DateTime.UtcNow,
            LogLevel = logLevel.ToString(),
            Category = categoryName,
            Message = formatter(state, exception),
            Exception = exception ?? new Exception("No exception provided")
        }));

        client.PublishEvent(new EventGridEvent(
                subject: "Microservice/Error",
                eventType: "ErroLog",
                dataVersion: "1.0",
                data: body
            ));
    }
}
