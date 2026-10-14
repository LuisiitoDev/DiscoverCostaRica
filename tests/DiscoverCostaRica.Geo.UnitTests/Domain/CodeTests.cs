using DiscoverCostaRica.Geo.Domain;
using DiscoverCostaRica.Geo.Domain.ValueObjects;

namespace DiscoverCostaRica.Geo.UnitTests.Domain;

public sealed class CodeTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    public void ProvinceCode_AcceptsTheSevenProvinces(int value)
    {
        Assert.True(ProvinceCode.TryCreate(value, out var code));
        Assert.Equal(value, code.Value);
        Assert.Equal(value.ToString(System.Globalization.CultureInfo.InvariantCulture), code.ToString());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(8)]
    [InlineData(-1)]
    public void ProvinceCode_RejectsAnythingElse(int value)
    {
        Assert.False(ProvinceCode.TryCreate(value, out _));
        Assert.Throws<ArgumentOutOfRangeException>(() => ProvinceCode.Create(value));
    }

    [Fact]
    public void CantonCode_IsEqualOnlyWithinTheSameProvince()
    {
        var sanJose = CantonCode.Create(ProvinceCode.Create(1), 1);
        var alajuela = CantonCode.Create(ProvinceCode.Create(2), 1);

        Assert.Equal(CantonCode.Create(ProvinceCode.Create(1), 1), sanJose);
        Assert.NotEqual(sanJose, alajuela);
        Assert.Equal("1-1", sanJose.ToString());
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(9, 1)]
    public void CantonCode_TryCreate_RejectsInvalidParts(int province, int canton) =>
        Assert.False(CantonCode.TryCreate(province, canton, out _));

    [Fact]
    public void CantonCode_Create_RejectsANonPositiveNumber() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => CantonCode.Create(ProvinceCode.Create(1), 0));

    [Fact]
    public void DistrictCode_CarriesItsCanton()
    {
        Assert.True(DistrictCode.TryCreate(2, 10, 7, out var code));
        Assert.Equal(CantonCode.Create(ProvinceCode.Create(2), 10), code.Canton);
        Assert.Equal(7, code.District);
        Assert.Equal("2-10-7", code.ToString());
    }

    [Theory]
    [InlineData(2, 10, 0)]
    [InlineData(2, 0, 7)]
    [InlineData(8, 10, 7)]
    public void DistrictCode_TryCreate_RejectsInvalidParts(int province, int canton, int district) =>
        Assert.False(DistrictCode.TryCreate(province, canton, district, out _));

    [Fact]
    public void DistrictCode_Create_RejectsANonPositiveNumber() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => DistrictCode.Create(CantonCode.Create(ProvinceCode.Create(1), 1), 0));

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Entities_RejectBlankNames(string name)
    {
        var canton = CantonCode.Create(ProvinceCode.Create(1), 1);

        Assert.ThrowsAny<ArgumentException>(() => Province.Create(ProvinceCode.Create(1), name));
        Assert.ThrowsAny<ArgumentException>(() => Canton.Create(canton, name));
        Assert.ThrowsAny<ArgumentException>(() => District.Create(DistrictCode.Create(canton, 1), name));
    }

    [Fact]
    public void Entities_RejectMissingCodes()
    {
        Assert.Throws<ArgumentNullException>(() => Province.Create(null!, "San José"));
        Assert.Throws<ArgumentNullException>(() => Canton.Create(null!, "San José"));
        Assert.Throws<ArgumentNullException>(() => District.Create(null!, "Carmen"));
        Assert.Throws<ArgumentNullException>(() => CantonCode.Create(null!, 1));
        Assert.Throws<ArgumentNullException>(() => DistrictCode.Create(null!, 1));
    }
}
