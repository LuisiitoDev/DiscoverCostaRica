using DiscoverCostaRica.Function.LogConsumer.Models;

namespace DiscoverCostaRica.Function.LogConsumer.Interfaces;

public interface IMongoService
{
    Task Log(LogModel log);
}
