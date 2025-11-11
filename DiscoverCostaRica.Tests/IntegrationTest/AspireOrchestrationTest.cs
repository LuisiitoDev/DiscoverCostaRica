using DiscoverCostaRica.Tests.Infraestructure;

namespace DiscoverCostaRica.Tests.IntegrationTest;

/// <summary>
/// Integration tests that validate the .NET Aspire orchestration and service discovery
/// </summary>
public class AspireOrchestrationTest
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

    [Fact]
    public async Task AspireApp_StartsSuccessfully()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);

        // Act
        await app.StartAsync(cancellationToken);

        // Assert - if we reach here without exception, app started successfully
        Assert.NotNull(app);
    }

    [Fact]
    public async Task AspireApp_AllMicroservices_BecomeHealthy()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var serviceNames = new[]
        {
            "discovercostarica-beaches-api",
            "discovercostarica-volcanoservice-api",
            "discovercostarica-cultureserice-api",
            "discovercostarica-geoservice-api"
        };

        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act & Assert - Wait for each service to become healthy
        foreach (var serviceName in serviceNames)
        {
            await app.ResourceNotifications.WaitForResourceHealthyAsync(serviceName, cancellationToken)
                .WaitAsync(DefaultTimeout, cancellationToken);
        }
    }

    [Fact]
    public async Task AspireApp_Gateway_BecomeHealthy()
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

    [Fact]
    public async Task AspireApp_AllServices_HaveHttpClients()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var serviceNames = new[]
        {
            "discovercostarica-beaches-api",
            "discovercostarica-volcanoservice-api",
            "discovercostarica-cultureserice-api",
            "discovercostarica-geoservice-api",
            "gateway"
        };

        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act & Assert - Create HTTP client for each service
        foreach (var serviceName in serviceNames)
        {
            await app.ResourceNotifications.WaitForResourceHealthyAsync(serviceName, cancellationToken)
                .WaitAsync(DefaultTimeout, cancellationToken);
            
            using var httpClient = app.CreateHttpClient(serviceName);
            Assert.NotNull(httpClient);
            Assert.NotNull(httpClient.BaseAddress);
        }
    }

    [Fact]
    public async Task AspireApp_Services_CanCommunicate()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Wait for all services
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-beaches-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        await app.ResourceNotifications.WaitForResourceHealthyAsync("discovercostarica-geoservice-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        // Act - Test that we can reach both services independently
        using var beachClient = app.CreateHttpClient("discovercostarica-beaches-api");
        using var geoClient = app.CreateHttpClient("discovercostarica-geoservice-api");

        using var beachResponse = await beachClient.GetAsync("/health", cancellationToken);
        using var geoResponse = await geoClient.GetAsync("/health", cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, beachResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, geoResponse.StatusCode);
    }

    [Fact]
    public async Task AspireApp_Gateway_RoutesToAllServices()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var routes = new Dictionary<string, string>
        {
            { "beaches", "discovercostarica-beaches-api" },
            { "volcano", "discovercostarica-volcanoservice-api" },
            { "culture", "discovercostarica-cultureserice-api" },
            { "geo", "discovercostarica-geoservice-api" }
        };

        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Wait for gateway and all backend services
        await app.ResourceNotifications.WaitForResourceHealthyAsync("gateway", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        
        foreach (var serviceName in routes.Values)
        {
            await app.ResourceNotifications.WaitForResourceHealthyAsync(serviceName, cancellationToken)
                .WaitAsync(DefaultTimeout, cancellationToken);
        }

        // Act & Assert - Gateway should route to each service
        using var gatewayClient = app.CreateHttpClient("gateway");
        
        foreach (var route in routes.Keys)
        {
            using var response = await gatewayClient.GetAsync($"/api/{route}", cancellationToken);
            
            // Should get some response (even if 401/404), not a gateway error
            Assert.True(
                response.StatusCode != HttpStatusCode.BadGateway && 
                response.StatusCode != HttpStatusCode.ServiceUnavailable,
                $"Gateway failed to route to {route}: {response.StatusCode}"
            );
        }
    }

    [Fact]
    public async Task AspireApp_HealthEndpoints_WorkForAllServices()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var serviceNames = new[]
        {
            "discovercostarica-beaches-api",
            "discovercostarica-volcanoservice-api",
            "discovercostarica-cultureserice-api",
            "discovercostarica-geoservice-api"
        };

        await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        // Act & Assert - Check health endpoint for each service
        foreach (var serviceName in serviceNames)
        {
            await app.ResourceNotifications.WaitForResourceHealthyAsync(serviceName, cancellationToken)
                .WaitAsync(DefaultTimeout, cancellationToken);
            
            using var httpClient = app.CreateHttpClient(serviceName);
            using var response = await httpClient.GetAsync("/health", cancellationToken);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
