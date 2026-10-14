using System.Diagnostics.CodeAnalysis;

namespace DiscoverCostaRica.Volcano.Domain;

/// <summary>
/// Where a volcano is, by official codes. Names belong to the Geo service; Volcano keeps only codes.
/// The district is optional because some volcanoes span districts.
/// </summary>
public sealed record LocationCode
{
    private LocationCode(int provinceId, int cantonId, int? districtId)
    {
        ProvinceId = provinceId;
        CantonId = cantonId;
        DistrictId = districtId;
    }

    public int ProvinceId { get; }

    public int CantonId { get; }

    public int? DistrictId { get; }

    public static bool TryCreate(int provinceId, int cantonId, int? districtId, [NotNullWhen(true)] out LocationCode? code)
    {
        code = provinceId > 0 && cantonId > 0 && districtId is null or > 0
            ? new LocationCode(provinceId, cantonId, districtId)
            : null;
        return code is not null;
    }

    public static LocationCode Create(int provinceId, int cantonId, int? districtId = null) =>
        TryCreate(provinceId, cantonId, districtId, out var code)
            ? code
            : throw new ArgumentOutOfRangeException(nameof(provinceId), "Location codes are positive numbers.");
}
