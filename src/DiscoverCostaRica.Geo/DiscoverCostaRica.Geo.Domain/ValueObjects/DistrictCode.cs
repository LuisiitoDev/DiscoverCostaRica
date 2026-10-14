using System.Diagnostics.CodeAnalysis;

namespace DiscoverCostaRica.Geo.Domain.ValueObjects;

/// <summary>District code. The number is unique only inside its canton, so the canton code travels with it.</summary>
public sealed record DistrictCode
{
    private DistrictCode(CantonCode canton, int district)
    {
        Canton = canton;
        District = district;
    }

    public CantonCode Canton { get; }

    public int District { get; }

    public static bool TryCreate(int province, int canton, int district, [NotNullWhen(true)] out DistrictCode? code)
    {
        code = district > 0 && CantonCode.TryCreate(province, canton, out var cantonCode) ? new DistrictCode(cantonCode, district) : null;
        return code is not null;
    }

    public static DistrictCode Create(CantonCode canton, int district)
    {
        ArgumentNullException.ThrowIfNull(canton);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(district);
        return new DistrictCode(canton, district);
    }

    public override string ToString() => $"{Canton}-{District}";
}
