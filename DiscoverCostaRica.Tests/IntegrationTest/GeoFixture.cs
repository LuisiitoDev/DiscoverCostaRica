using DiscoverCostaRica.Tests.Constants.Services;
using DiscoverCostaRica.Tests.Infraestructure;

namespace DiscoverCostaRica.Tests.IntegrationTest;

public class GeoFixture(Fixture<GeoServiceType> fixture) : IClassFixture<Fixture<GeoServiceType>>
{
    [Fact]
    public async Task GetProvinces_ReturnsOk()
    {
        // Arrange
        using var http = fixture.CreateClient();
        // Act
        using var response = await http.GetAsync("/api/v1/geo/provinces");
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
