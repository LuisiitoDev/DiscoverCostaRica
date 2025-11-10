using DiscoverCostaRica.Tests.Infraestructure;

namespace DiscoverCostaRica.Tests.IntegrationTest;

public class GeoIntegrationTest
{
    [Theory]
    [InlineData("/api/v1/geo/provinces")]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync( cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("discovercostarica-geoservice-api");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-geoservice-api", cancellationToken);
        using var response = await httpClient.GetAsync(url, cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
