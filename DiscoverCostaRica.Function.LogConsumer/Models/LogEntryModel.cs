namespace DiscoverCostaRica.Function.LogConsumer.Models;

public record LogEntryModel
{
    public required DateTime Timestamp { get; set; }
    public required string LogLevel { get; set; }
    public required string Category { get; set; }
    public required string Message { get; set; }
    public required Exception Exception { get; set; }
}
