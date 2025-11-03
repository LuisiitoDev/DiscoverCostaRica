using DiscoverCostaRica.Function.LogConsumer.Interfaces;
using DiscoverCostaRica.Function.LogConsumer.Models;
using MongoDB.Driver;

namespace DiscoverCostaRica.Function.LogConsumer.Services;

public class MongoService : IMongoService
{
    private readonly IMongoCollection<LogModel> _collection;

    public MongoService()
    {
        var client = new MongoClient(Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING"));
        var database = client.GetDatabase(Environment.GetEnvironmentVariable("MONGO_DATABASE"));
        _collection = database.GetCollection<LogModel>(Environment.GetEnvironmentVariable("MONGO_COLLECTION"));
    }

    public async Task Log(LogModel log)
    {
        await _collection.InsertOneAsync(log);
    }
}
