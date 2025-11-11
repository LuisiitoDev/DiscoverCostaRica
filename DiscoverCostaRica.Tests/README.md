# DiscoverCostaRica Integration Tests

This project contains comprehensive functional integration tests for the Discover Costa Rica microservices application built with .NET Aspire.

## Overview

The test suite validates the entire distributed application orchestrated by .NET Aspire, including:
- All microservices (Beaches, Volcano, Culture, Geo)
- API Gateway (YARP)
- Service discovery and orchestration
- Health endpoints
- Inter-service communication

## Test Structure

### Infrastructure (`Infraestructure/`)

- **DistributedApplicationTestFactory.cs**: Factory for creating and configuring the distributed application for testing
- **TestEndpoints.cs**: Helper class for managing test endpoints

### Integration Tests (`IntegrationTest/`)

#### Service-Specific Tests

1. **BeachesIntegrationTest.cs** (5 tests)
   - Health and liveness endpoint validation
   - GET /api/v1/beaches endpoint
   - Content-Type validation

2. **VolcanoIntegrationTest.cs** (6 tests)
   - Health and liveness endpoint validation
   - GET /api/v1/volcanoes endpoints
   - GET by ID and by province
   - Content-Type validation

3. **CultureIntegrationTest.cs** (6 tests)
   - Health and liveness endpoint validation
   - GET /api/v1/traditions/tradition endpoint
   - GET /api/v1/traditions/dish endpoint
   - Content-Type validation

4. **GeoIntegrationTest.cs** (6 tests)
   - Health and liveness endpoint validation
   - GET /api/v1/geo/provinces endpoint
   - GET cantons by province
   - GET districts by canton
   - Content-Type validation

#### Gateway Tests

5. **GatewayIntegrationTest.cs** (7 tests)
   - Gateway health validation
   - Routing to all backend services
   - Service-specific route validation

#### Orchestration Tests

6. **AspireOrchestrationTest.cs** (7 tests)
   - Application startup validation
   - All microservices health validation
   - HTTP client creation for all services
   - Service-to-service communication
   - Gateway routing validation

**Total: 37 functional integration tests**

## Running the Tests

### Prerequisites

- .NET 10.0 SDK (or the version specified in the project)
- Docker (for running dependencies like SQL Server, MongoDB)
- Azure resources configured (optional, depending on configuration)

### Run All Tests

```bash
dotnet test
```

### Run Specific Test Class

```bash
dotnet test --filter ClassName~BeachesIntegrationTest
```

### Run Tests with Detailed Output

```bash
dotnet test --verbosity detailed
```

### Run Tests in Parallel

```bash
dotnet test --parallel
```

## Test Patterns

### Basic Test Structure

```csharp
[Fact]
public async Task ServiceName_Endpoint_ExpectedBehavior()
{
    // Arrange
    var cancellationToken = CancellationToken.None;
    await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
    await app.StartAsync(cancellationToken);

    // Act
    using var httpClient = app.CreateHttpClient("service-name");
    await app.ResourceNotifications.WaitForResourceHealthyAsync("service-name", cancellationToken)
        .WaitAsync(DefaultTimeout, cancellationToken);
    using var response = await httpClient.GetAsync("/endpoint", cancellationToken);

    // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
}
```

### Key Components

1. **DistributedApplicationTestFactory**: Creates the Aspire application for testing
2. **CreateHttpClient**: Creates an HTTP client for a specific service
3. **WaitForResourceHealthyAsync**: Waits for a service to be healthy before testing
4. **DefaultTimeout**: Standard timeout (30-60 seconds) for operations

## Authentication Handling

The services use Microsoft Entra ID (formerly Azure AD) for authentication in production. For testing, authentication is mocked/bypassed:

- **Test Configuration**: The `appsettings.Testing.json` file contains empty EntraId configuration
- **Mock Handler**: `TestAuthenticationHandler` provides test claims without requiring real authentication
- **Environment Setup**: Tests set the environment to "Testing" to disable Entra ID authentication
- **Result**: Services skip authentication setup when EntraId TenantId is empty, allowing tests to access endpoints directly
- **Test Assertions**: Tests expect 200 OK responses instead of allowing 401 Unauthorized

This approach allows tests to validate service functionality without requiring Azure AD configuration or tokens.

## Service Names

The following service names are used in the AppHost and tests:

- `discovercostarica-beaches-api`: Beaches microservice
- `discovercostarica-volcanoservice-api`: Volcano microservice
- `discovercostarica-cultureserice-api`: Culture microservice
- `discovercostarica-geoservice-api`: Geo microservice
- `gateway`: YARP API Gateway
- `discovercostarica-functions`: Azure Functions for log processing

## CI/CD Integration

These tests are designed to run in CI/CD pipelines:

```yaml
# Example GitHub Actions workflow
- name: Run Integration Tests
  run: dotnet test --logger "trx;LogFileName=test-results.trx"
  
- name: Publish Test Results
  uses: dorny/test-reporter@v1
  if: always()
  with:
    name: Integration Test Results
    path: "**/test-results.trx"
    reporter: dotnet-trx
```

## Troubleshooting

### Tests Timeout

If tests timeout, increase the `DefaultTimeout` value or check:
- Docker containers are running
- Network connectivity
- Resource constraints (CPU, memory)

### Service Not Healthy

If a service fails health checks:
1. Check the service logs in the test output
2. Verify database connections
3. Check for missing configuration

### Connection Refused

If you get connection refused errors:
1. Ensure Docker is running
2. Check port conflicts
3. Verify the AppHost configuration

## Contributing

When adding new tests:

1. Follow the existing naming convention: `ServiceName_Endpoint_ExpectedBehavior`
2. Use appropriate timeout values
3. Handle authentication scenarios (200 OK or 401 Unauthorized)
4. Add both positive and negative test cases
5. Document any special setup requirements

## Resources

- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Aspire Testing Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/testing)
- [xUnit Documentation](https://xunit.net/)
- [YARP Documentation](https://microsoft.github.io/reverse-proxy/)
