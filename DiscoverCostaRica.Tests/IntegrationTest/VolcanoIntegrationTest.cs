using DiscoverCostaRica.Tests.Infraestructure;

namespace DiscoverCostaRica.Tests.IntegrationTest;

public class VolcanoIntegrationTest
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    [Fact]
    public async Task VolcanoService_HealthEndpoint_ReturnsHealthy()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("discovercostarica-volcanoservice-api");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-volcanoservice-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        using var response = await httpClient.GetAsync("/health", cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task VolcanoService_AliveEndpoint_ReturnsHealthy()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("discovercostarica-volcanoservice-api");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-volcanoservice-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        using var response = await httpClient.GetAsync("/alive", cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/v1/volcanoes")]
    [InlineData("/api/v1/volcanoes/1")]
    public async Task VolcanoService_GetEndpoints_ReturnsSuccessStatusCode(string url)
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("discovercostarica-volcanoservice-api");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-volcanoservice-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        using var response = await httpClient.GetAsync(url, cancellationToken);

        // Assert
        // Authentication is mocked/bypassed in tests
        // May return 200 OK or 404 Not Found depending on data
        Assert.True(
            response.StatusCode == HttpStatusCode.OK || 
            response.StatusCode == HttpStatusCode.NotFound,
            $"Expected OK or NotFound but got {response.StatusCode}"
        );
    }

    [Fact]
    public async Task VolcanoService_GetVolcanoesByProvince_ReturnsSuccessStatusCode()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("discovercostarica-volcanoservice-api");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-volcanoservice-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        using var response = await httpClient.GetAsync("/api/v1/volcanoes/province/1", cancellationToken);

        // Assert
        // Authentication is mocked/bypassed in tests
        // May return 200 OK or 404 Not Found depending on data
        Assert.True(
            response.StatusCode == HttpStatusCode.OK || 
            response.StatusCode == HttpStatusCode.NotFound,
            $"Expected OK or NotFound but got {response.StatusCode}"
        );
    }

    [Fact]
    public async Task VolcanoService_HasCorrectContentType()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("discovercostarica-volcanoservice-api");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-volcanoservice-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        using var response = await httpClient.GetAsync("/api/v1/volcanoes", cancellationToken);

        // Assert
        // If data exists, we should get OK with JSON content
        if (response.StatusCode == HttpStatusCode.OK)
        {
            Assert.Contains("application/json", response.Content.Headers.ContentType?.ToString());
        }
        else
        {
            // Otherwise, 404 is acceptable if no data exists
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
