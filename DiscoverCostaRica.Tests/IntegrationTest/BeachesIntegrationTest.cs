using DiscoverCostaRica.Tests.Infraestructure;

namespace DiscoverCostaRica.Tests.IntegrationTest;

public class BeachesIntegrationTest
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    [Fact]
    public async Task BeachService_HealthEndpoint_ReturnsHealthy()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("discovercostarica-beaches-api");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-beaches-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        using var response = await httpClient.GetAsync("/health", cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task BeachService_AliveEndpoint_ReturnsHealthy()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("discovercostarica-beaches-api");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-beaches-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        using var response = await httpClient.GetAsync("/alive", cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/v1/beaches")]
    public async Task BeachService_GetBeaches_ReturnsSuccessStatusCode(string url)
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("discovercostarica-beaches-api");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-beaches-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        using var response = await httpClient.GetAsync(url, cancellationToken);

        // Assert
        // Authentication is mocked/bypassed in tests, so we expect 200 OK
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task BeachService_HasCorrectContentType()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("discovercostarica-beaches-api");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-beaches-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        using var response = await httpClient.GetAsync("/api/v1/beaches", cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("application/json", response.Content.Headers.ContentType?.ToString());
    }
}
