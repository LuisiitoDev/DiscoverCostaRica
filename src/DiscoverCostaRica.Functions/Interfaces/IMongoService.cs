using DiscoverCostaRica.Functions.Models;

namespace DiscoverCostaRica.Functions.Interfaces;

public interface IMongoService
{
    Task Log(LogModel log);
}
