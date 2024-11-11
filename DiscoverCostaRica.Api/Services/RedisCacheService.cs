using DiscoverCostaRica.Api.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace DiscoverCostaRica.Api.Services;

public class RedisCacheService(IOptions<RedisConfiguration> configuration)
{
	private readonly Lazy<ConnectionMultiplexer> redis = new(() =>
	{
		return ConnectionMultiplexer.Connect(new ConfigurationOptions
		{
			EndPoints = { configuration.Value.Endpoint },
			User = "default",
			Password = configuration.Value.Password,
			AbortOnConnectFail = false
		});
	});
	private IDatabase Db => redis.Value.GetDatabase();

	public async Task SetAsync<TValue>(string key, TValue value)
	{
		await Db.StringSetAsync(key, System.Text.Json.JsonSerializer.Serialize(value));
	}

	public async Task<TResult> GetAsync<TResult>(string key)
	{
		var json = await Db.StringGetAsync(key);
		if (string.IsNullOrEmpty(json)) return default!;
		return System.Text.Json.JsonSerializer.Deserialize<TResult>(json!)!;
	}

	public async Task<bool> ContainsKeyAsync(string key)
	{
		return await Db.KeyExistsAsync(key);
	}

}