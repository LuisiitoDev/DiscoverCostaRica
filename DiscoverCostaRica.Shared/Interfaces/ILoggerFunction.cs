using DiscoverCostaRica.Shared.Logging;

namespace DiscoverCostaRica.Shared.Interfaces;

public interface ILoggerFunction
{
    Task SendLogger(LogEntryModel log);
}
