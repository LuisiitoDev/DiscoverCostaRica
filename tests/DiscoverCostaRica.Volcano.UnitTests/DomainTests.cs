using DiscoverCostaRica.Volcano.Domain;

namespace DiscoverCostaRica.Volcano.UnitTests;

public sealed class DomainTests
{
    [Theory]
    [InlineData(2, 10, 7)]
    [InlineData(2, 10, null)]
    public void LocationCode_AcceptsPositiveCodes(int province, int canton, int? district)
    {
        var code = LocationCode.Create(province, canton, district);

        Assert.Equal((province, canton, district), (code.ProvinceId, code.CantonId, code.DistrictId));
        Assert.Equal(LocationCode.Create(province, canton, district), code);
    }

    [Theory]
    [InlineData(0, 1, null)]
    [InlineData(1, 0, null)]
    [InlineData(1, 1, 0)]
    public void LocationCode_RejectsNonPositiveCodes(int province, int canton, int? district)
    {
        Assert.False(LocationCode.TryCreate(province, canton, district, out _));
        Assert.Throws<ArgumentOutOfRangeException>(() => LocationCode.Create(province, canton, district));
    }

    [Fact]
    public void Volcano_ValidatesItsData()
    {
        var location = LocationCode.Create(2, 10, 7);

        Assert.Equal("Arenal", Domain.Volcano.Create(1, "Arenal", "Stratovolcano.", location).Name);
        Assert.Throws<ArgumentOutOfRangeException>(() => Domain.Volcano.Create(0, "Arenal", "Stratovolcano.", location));
        Assert.ThrowsAny<ArgumentException>(() => Domain.Volcano.Create(1, " ", "Stratovolcano.", location));
        Assert.ThrowsAny<ArgumentException>(() => Domain.Volcano.Create(1, "Arenal", "", location));
        Assert.Throws<ArgumentNullException>(() => Domain.Volcano.Create(1, "Arenal", "Stratovolcano.", null!));
    }
}
