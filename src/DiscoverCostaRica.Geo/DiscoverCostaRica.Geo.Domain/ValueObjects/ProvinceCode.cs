using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace DiscoverCostaRica.Geo.Domain.ValueObjects;

/// <summary>Official province code. Costa Rica has seven provinces, numbered 1 (San José) to 7 (Limón).</summary>
public sealed record ProvinceCode
{
    public const int Min = 1;
    public const int Max = 7;

    private ProvinceCode(int value) => Value = value;

    public int Value { get; }

    public static bool TryCreate(int value, [NotNullWhen(true)] out ProvinceCode? code)
    {
        code = value is >= Min and <= Max ? new ProvinceCode(value) : null;
        return code is not null;
    }

    public static ProvinceCode Create(int value) =>
        TryCreate(value, out var code)
            ? code
            : throw new ArgumentOutOfRangeException(nameof(value), value, $"A province code is between {Min} and {Max}.");

    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
