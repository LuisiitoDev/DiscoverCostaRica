using DiscoverCostaRica.Functions.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace DiscoverCostaRica.Functions.Services;

public class CacheService(IOptions<CacheConfiguration> options)
{
    private readonly Lazy<ConnectionMultiplexer> connection = new(() =>
    {
        return ConnectionMultiplexer.Connect(new ConfigurationOptions()
        {
            EndPoints = { options.Value.Endpoint },
            User = "default",
            Password = options.Value.Password,
            AbortOnConnectFail = false,
        });
    });

    private IDatabase Db => connection.Value.GetDatabase();

    public async Task AddAsync<TValue>(string key, TValue value)
    {
        if (Db.KeyExists(key)) await Db.KeyDeleteAsync(key);
        await Db.StringSetAsync(key, JsonSerializer.Serialize(value));
    }
}
