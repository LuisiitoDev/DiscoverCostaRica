using System.Diagnostics.CodeAnalysis;

namespace DiscoverCostaRica.Geo.Domain.ValueObjects;

/// <summary>
/// Canton code. The number is unique only inside its province (canton 1 exists in every province),
/// so a canton is identified by both.
/// </summary>
public sealed record CantonCode
{
    private CantonCode(ProvinceCode province, int canton)
    {
        Province = province;
        Canton = canton;
    }

    public ProvinceCode Province { get; }

    public int Canton { get; }

    public static bool TryCreate(int province, int canton, [NotNullWhen(true)] out CantonCode? code)
    {
        code = canton > 0 && ProvinceCode.TryCreate(province, out var provinceCode) ? new CantonCode(provinceCode, canton) : null;
        return code is not null;
    }

    public static CantonCode Create(ProvinceCode province, int canton)
    {
        ArgumentNullException.ThrowIfNull(province);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(canton);
        return new CantonCode(province, canton);
    }

    public override string ToString() => $"{Province}-{Canton}";
}
