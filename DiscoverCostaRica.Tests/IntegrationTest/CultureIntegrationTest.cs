using DiscoverCostaRica.Tests.Infraestructure;

namespace DiscoverCostaRica.Tests.IntegrationTest;

public class CultureIntegrationTest
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    [Fact]
    public async Task CultureService_HealthEndpoint_ReturnsHealthy()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("discovercostarica-cultureserice-api");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-cultureserice-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        using var response = await httpClient.GetAsync("/health", cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CultureService_AliveEndpoint_ReturnsHealthy()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("discovercostarica-cultureserice-api");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-cultureserice-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        using var response = await httpClient.GetAsync("/alive", cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/v1/traditions/tradition")]
    [InlineData("/api/v1/traditions/dish")]
    public async Task CultureService_GetEndpoints_ReturnsSuccessStatusCode(string url)
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("discovercostarica-cultureserice-api");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-cultureserice-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        using var response = await httpClient.GetAsync(url, cancellationToken);

        // Assert
        // Authentication is mocked/bypassed in tests, so we expect 200 OK
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CultureService_GetTraditions_HasCorrectContentType()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("discovercostarica-cultureserice-api");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-cultureserice-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        using var response = await httpClient.GetAsync("/api/v1/traditions/tradition", cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("application/json", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task CultureService_GetDishes_HasCorrectContentType()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("discovercostarica-cultureserice-api");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-cultureserice-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        using var response = await httpClient.GetAsync("/api/v1/traditions/dish", cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("application/json", response.Content.Headers.ContentType?.ToString());
    }
}
