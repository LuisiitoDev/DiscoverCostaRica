using DiscoverCostaRica.AppHost.Constants;

namespace DiscoverCostaRica.Tests.Constants.Services;

public struct GeoServiceType : IBaseServiceType
{
    public readonly string Name => Microservices.Geo;
}
