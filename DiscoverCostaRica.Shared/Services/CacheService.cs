using DiscoverCostaRica.Shared.Attributes;
using DiscoverCostaRica.Shared.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace DiscoverCostaRica.Shared.Services
{
    [TransientService]
    public class CacheService(IConnectionMultiplexer multiplexer) : ICacheService
    {
        private readonly Lazy<IDatabase> _database = new(() => multiplexer.GetDatabase());
        public async Task<TSource?> Get<TSource>(string key, CancellationToken cancellationToken)
        {
            var value = await _database.Value.StringGetAsync(key);
            if (value == RedisValue.Null) return default;
            return JsonSerializer.Deserialize<TSource>(value.ToString());
        }

        public async Task Set<TSource>(string key, TSource source, CancellationToken cancellationToken)
        {
            var json = JsonSerializer.Serialize(source);
            await _database.Value.StringSetAsync(key, json);
        }
    }
}