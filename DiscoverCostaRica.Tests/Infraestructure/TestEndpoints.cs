using System.Text.Json;
using Xunit.Abstractions;

namespace DiscoverCostaRica.Tests.Infraestructure;

public class TestEndpoints : IXunitSerializable
{
    public TestEndpoints() { }

    public TestEndpoints(Dictionary<string, List<string>> resourceEndpoints)
    {
        ResourceEndpoints = resourceEndpoints;
    }

    public Dictionary<string, List<string>> ResourceEndpoints { get; set; } = [];

    public void Deserialize(IXunitSerializationInfo info)
    {
        ResourceEndpoints = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(info.GetValue<string>(nameof(ResourceEndpoints)))!;
    }

    public void Serialize(IXunitSerializationInfo info)
    {
        info.AddValue(nameof(ResourceEndpoints), JsonSerializer.Serialize(ResourceEndpoints));
    }
}
