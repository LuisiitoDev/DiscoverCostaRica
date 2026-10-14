using DiscoverCostaRica.Geo.Domain.ValueObjects;

namespace DiscoverCostaRica.Geo.Domain;

/// <summary>Second-level division. Belongs to exactly one province, which is part of its code.</summary>
public sealed class Canton
{
    private Canton(CantonCode code, string name)
    {
        Code = code;
        Name = name;
    }

    public CantonCode Code { get; }

    public string Name { get; }

    public static Canton Create(CantonCode code, string name)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new Canton(code, name);
    }
}
