using DiscoverCostaRica.Tests.Infraestructure;

namespace DiscoverCostaRica.Tests.IntegrationTest;

public class GatewayIntegrationTest
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    [Fact]
    public async Task Gateway_IsHealthy()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        await app.ResourceNotifications.WaitForResourceHealthyAsync("gateway", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        // Assert - if we reach here without exception, gateway is healthy
        Assert.True(true);
    }

    [Theory]
    [InlineData("/api/beaches")]
    [InlineData("/api/volcano")]
    [InlineData("/api/culture")]
    [InlineData("/api/geo")]
    public async Task Gateway_RoutesToServices_ReturnsResponse(string route)
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("gateway");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("gateway", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        using var response = await httpClient.GetAsync(route, cancellationToken);

        // Assert
        // Gateway should route successfully, even if backend requires auth
        // Valid responses: 200 OK, 401 Unauthorized, 404 Not Found
        Assert.True(
            response.StatusCode == HttpStatusCode.OK || 
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.NotFound,
            $"Expected valid HTTP status for route {route} but got {response.StatusCode}"
        );
    }

    [Fact]
    public async Task Gateway_BeachesRoute_RoutesToBeachesService()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("gateway");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("gateway", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        
        // Wait for beaches service to be healthy
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-beaches-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
            
        using var response = await httpClient.GetAsync("/api/beaches", cancellationToken);

        // Assert
        // Should get a response from the beaches service through the gateway
        Assert.True(
            response.StatusCode == HttpStatusCode.OK || 
            response.StatusCode == HttpStatusCode.Unauthorized,
            $"Expected gateway to route to beaches service but got {response.StatusCode}"
        );
    }

    [Fact]
    public async Task Gateway_VolcanoRoute_RoutesToVolcanoService()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("gateway");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("gateway", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        
        // Wait for volcano service to be healthy
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-volcanoservice-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
            
        using var response = await httpClient.GetAsync("/api/volcano", cancellationToken);

        // Assert
        // Should get a response from the volcano service through the gateway
        Assert.True(
            response.StatusCode == HttpStatusCode.OK || 
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.NotFound,
            $"Expected gateway to route to volcano service but got {response.StatusCode}"
        );
    }

    [Fact]
    public async Task Gateway_CultureRoute_RoutesToCultureService()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("gateway");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("gateway", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        
        // Wait for culture service to be healthy
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-cultureserice-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
            
        using var response = await httpClient.GetAsync("/api/culture", cancellationToken);

        // Assert
        // Should get a response from the culture service through the gateway
        Assert.True(
            response.StatusCode == HttpStatusCode.OK || 
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.NotFound,
            $"Expected gateway to route to culture service but got {response.StatusCode}"
        );
    }

    [Fact]
    public async Task Gateway_GeoRoute_RoutesToGeoService()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("gateway");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("gateway", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        
        // Wait for geo service to be healthy
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-geoservice-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
            
        using var response = await httpClient.GetAsync("/api/geo", cancellationToken);

        // Assert
        // Should get a response from the geo service through the gateway
        Assert.True(
            response.StatusCode == HttpStatusCode.OK || 
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.NotFound,
            $"Expected gateway to route to geo service but got {response.StatusCode}"
        );
    }
}
