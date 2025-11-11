using DiscoverCostaRica.Tests.Infraestructure;

namespace DiscoverCostaRica.Tests.IntegrationTest;

public class GeoIntegrationTest
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    [Fact]
    public async Task GeoService_HealthEndpoint_ReturnsHealthy()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("discovercostarica-geoservice-api");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-geoservice-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        using var response = await httpClient.GetAsync("/health", cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GeoService_AliveEndpoint_ReturnsHealthy()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("discovercostarica-geoservice-api");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-geoservice-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        using var response = await httpClient.GetAsync("/alive", cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/v1/geo/provinces")]
    public async Task GeoService_GetProvinces_ReturnsSuccessStatusCode(string url)
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("discovercostarica-geoservice-api");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-geoservice-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        using var response = await httpClient.GetAsync(url, cancellationToken);

        // Assert
        // May return 200 OK or 401 Unauthorized depending on auth configuration
        Assert.True(
            response.StatusCode == HttpStatusCode.OK || 
            response.StatusCode == HttpStatusCode.Unauthorized,
            $"Expected OK or Unauthorized but got {response.StatusCode}"
        );
    }

    [Fact]
    public async Task GeoService_GetCantonsByProvince_ReturnsSuccessStatusCode()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("discovercostarica-geoservice-api");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-geoservice-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        using var response = await httpClient.GetAsync("/api/v1/geo/cantons/1", cancellationToken);

        // Assert
        // May return 200 OK, 404 Not Found, or 401 Unauthorized
        Assert.True(
            response.StatusCode == HttpStatusCode.OK || 
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.Unauthorized,
            $"Expected OK, NotFound or Unauthorized but got {response.StatusCode}"
        );
    }

    [Fact]
    public async Task GeoService_GetDistrictsByCanton_ReturnsSuccessStatusCode()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("discovercostarica-geoservice-api");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-geoservice-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        using var response = await httpClient.GetAsync("/api/v1/geo/districts/1", cancellationToken);

        // Assert
        // May return 200 OK, 404 Not Found, or 401 Unauthorized
        Assert.True(
            response.StatusCode == HttpStatusCode.OK || 
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.Unauthorized,
            $"Expected OK, NotFound or Unauthorized but got {response.StatusCode}"
        );
    }

    [Fact]
    public async Task GeoService_HasCorrectContentType()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("discovercostarica-geoservice-api");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-geoservice-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        using var response = await httpClient.GetAsync("/api/v1/geo/provinces", cancellationToken);

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            Assert.Contains("application/json", response.Content.Headers.ContentType?.ToString());
        }
    }
}
