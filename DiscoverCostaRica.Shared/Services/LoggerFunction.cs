using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Shared.Interfaces;
using DiscoverCostaRica.Shared.Logging;
using MongoDB.Driver;

namespace DiscoverCostaRica.Shared.Services;

[SingletonService]
public class LoggerFunction : ILoggerFunction
{
    private readonly IMongoCollection<LogEntryModel> _collection;

    public LoggerFunction(IMongoClient client)
    {
        var database = client.GetDatabase("discovercotarica");
        _collection = database.GetCollection<LogEntryModel>("logs");
    }

    public async Task SendLogger(LogEntryModel log)
    {
        await _collection.InsertOneAsync(log);
    }
}
