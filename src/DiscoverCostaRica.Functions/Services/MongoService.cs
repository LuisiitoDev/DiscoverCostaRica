using DiscoverCostaRica.Functions.Interfaces;
using DiscoverCostaRica.Functions.Models;
using MongoDB.Driver;

namespace DiscoverCostaRica.Functions.Services;

public class MongoService : IMongoService
{
    private readonly IMongoCollection<LogModel> _collection;

    public MongoService(IMongoClient client)
    {
        var database = client.GetDatabase(Environment.GetEnvironmentVariable("MONGO_DATABASE"));
        _collection = database.GetCollection<LogModel>(Environment.GetEnvironmentVariable("MONGO_COLLECTION"));
    }

    public async Task Log(LogModel log)
    {
        await _collection.InsertOneAsync(log);
    }
}
