using DiscoverCostaRica.Geo.Domain.ValueObjects;

namespace DiscoverCostaRica.Geo.Domain;

/// <summary>Third-level division. Belongs to exactly one canton, which is part of its code.</summary>
public sealed class District
{
    private District(DistrictCode code, string name)
    {
        Code = code;
        Name = name;
    }

    public DistrictCode Code { get; }

    public string Name { get; }

    public static District Create(DistrictCode code, string name)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new District(code, name);
    }
}
