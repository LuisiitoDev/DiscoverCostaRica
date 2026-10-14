using DiscoverCostaRica.Geo.Domain.ValueObjects;

namespace DiscoverCostaRica.Geo.Domain;

/// <summary>First-level administrative division.</summary>
public sealed class Province
{
    private Province(ProvinceCode code, string name)
    {
        Code = code;
        Name = name;
    }

    public ProvinceCode Code { get; }

    public string Name { get; }

    public static Province Create(ProvinceCode code, string name)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new Province(code, name);
    }
}
