# Integration Test Summary for Discover Costa Rica

## Overview

This document provides a comprehensive overview of the functional integration tests created for the Discover Costa Rica .NET Aspire application.

## Test Architecture

### Technology Stack
- **Testing Framework**: xUnit
- **Aspire Testing**: Aspire.Hosting.Testing v9.5.0
- **Target Framework**: .NET 10.0
- **Test Pattern**: Functional Integration Tests

### Design Principles

1. **Aspire-First Testing**: Tests use .NET Aspire's built-in testing infrastructure
2. **Real Service Testing**: Tests spin up actual services, not mocks
3. **Health-Driven Validation**: Tests wait for services to be healthy before making requests
4. **Authentication Aware**: Tests handle both authenticated and non-authenticated scenarios
5. **Isolation**: Each test can run independently
6. **Comprehensive Coverage**: Tests cover all microservices, gateway, and orchestration

## Test Categories

### 1. Service Health Tests

Each microservice has health endpoint tests to validate:
- `/health` endpoint returns 200 OK
- `/alive` endpoint returns 200 OK
- Service becomes healthy in the Aspire orchestration

**Services Covered:**
- Beaches Service
- Volcano Service
- Culture Service
- Geo Service

### 2. API Endpoint Tests

Each microservice has endpoint-specific tests:

#### Beaches Service
- `GET /api/v1/beaches` - List all beaches

#### Volcano Service
- `GET /api/v1/volcanoes` - List all volcanoes
- `GET /api/v1/volcanoes/{id}` - Get volcano by ID
- `GET /api/v1/volcanoes/province/{provinceId}` - Get volcanoes by province

#### Culture Service
- `GET /api/v1/traditions/tradition` - Get traditions
- `GET /api/v1/traditions/dish` - Get dishes

#### Geo Service
- `GET /api/v1/geo/provinces` - Get provinces
- `GET /api/v1/geo/cantons/{provinceId}` - Get cantons by province
- `GET /api/v1/geo/districts/{cantonId}` - Get districts by canton

### 3. Gateway (YARP) Tests

Tests validate the API Gateway routing:
- Gateway becomes healthy
- Routes correctly to all backend services
- Transforms paths according to configuration:
  - `/api/beaches` → Beaches Service `/api/v1/`
  - `/api/volcano` → Volcano Service `/api/v1/`
  - `/api/culture` → Culture Service `/api/v1/`
  - `/api/geo` → Geo Service `/api/v1/`

### 4. Aspire Orchestration Tests

Tests validate the .NET Aspire orchestration:
- Application starts successfully
- All microservices become healthy
- HTTP clients can be created for all services
- Services can communicate through the orchestration
- Gateway routes to all services correctly
- Health endpoints work across all services

## Test Statistics

| Test Class | Test Count | Focus Area |
|------------|-----------|------------|
| BeachesIntegrationTest | 5 | Beaches Service |
| VolcanoIntegrationTest | 6 | Volcano Service |
| CultureIntegrationTest | 6 | Culture Service |
| GeoIntegrationTest | 6 | Geo Service |
| GatewayIntegrationTest | 7 | API Gateway |
| AspireOrchestrationTest | 7 | Aspire Orchestration |
| **TOTAL** | **37** | **Full Stack** |

## Test Execution Flow

```
1. Test Method Invoked
   ↓
2. DistributedApplicationTestFactory.CreateAsync()
   ↓
3. App.StartAsync() - Starts all Aspire resources
   ↓
4. WaitForResourceHealthyAsync() - Wait for service to be healthy
   ↓
5. CreateHttpClient() - Create HTTP client for service
   ↓
6. Make HTTP Request to service
   ↓
7. Assert Response
   ↓
8. Cleanup (using/await using pattern)
```

## Authentication Handling

The application uses Microsoft Entra ID authentication in production. Tests bypass authentication using a mock configuration:

### Test Authentication Implementation

1. **Empty Configuration**: `appsettings.Testing.json` contains empty EntraId values
2. **Environment Setup**: Tests set `ASPNETCORE_ENVIRONMENT` to "Testing"
3. **Service Behavior**: Services check if EntraId TenantId is configured; if empty, authentication is skipped
4. **Test Handler**: `TestAuthenticationHandler` provides mock authentication with test claims

```csharp
// Tests now expect successful responses
Assert.Equal(HttpStatusCode.OK, response.StatusCode);
```

This approach allows tests to validate service functionality without requiring real Azure AD tokens:
- **Test Environment**: Authentication disabled via empty configuration → 200 OK
- **Production Environment**: Full Entra ID authentication enabled

## Key Test Patterns

### Pattern 1: Basic Service Health Check

```csharp
[Fact]
public async Task ServiceName_HealthEndpoint_ReturnsHealthy()
{
    var cancellationToken = CancellationToken.None;
    await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
    await app.StartAsync(cancellationToken);

    using var httpClient = app.CreateHttpClient("service-name");
    await app.ResourceNotifications.WaitForResourceHealthyAsync("service-name", cancellationToken)
        .WaitAsync(DefaultTimeout, cancellationToken);
    using var response = await httpClient.GetAsync("/health", cancellationToken);

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
}
```

### Pattern 2: API Endpoint Test with Mock Authentication

```csharp
[Theory]
[InlineData("/api/v1/endpoint")]
public async Task ServiceName_GetEndpoint_ReturnsSuccessStatusCode(string url)
{
    var cancellationToken = CancellationToken.None;
    await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
    await app.StartAsync(cancellationToken);

    using var httpClient = app.CreateHttpClient("service-name");
    await app.ResourceNotifications.WaitForResourceHealthyAsync("service-name", cancellationToken)
        .WaitAsync(DefaultTimeout, cancellationToken);
    using var response = await httpClient.GetAsync(url, cancellationToken);

    // Authentication is mocked/bypassed in tests
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
}
```

### Pattern 3: Gateway Routing Test

```csharp
[Fact]
public async Task Gateway_RouteToService_RoutesSuccessfully()
{
    var cancellationToken = CancellationToken.None;
    await using var app = await DistributedApplicationTestFactory.CreateAsync(cancellationToken);
    await app.StartAsync(cancellationToken);

    using var httpClient = app.CreateHttpClient("gateway");
    await app.ResourceNotifications.WaitForResourceHealthyAsync("gateway", cancellationToken)
        .WaitAsync(DefaultTimeout, cancellationToken);
    await app.ResourceNotifications.WaitForResourceHealthyAsync("backend-service", cancellationToken)
        .WaitAsync(DefaultTimeout, cancellationToken);
        
    using var response = await httpClient.GetAsync("/api/route", cancellationToken);

    // Authentication is mocked/bypassed in tests
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
}
```

## Infrastructure Components

### DistributedApplicationTestFactory

The factory creates a configured Aspire application for testing with mock authentication:

```csharp
public static async Task<DistributedApplication> CreateAsync(CancellationToken cancellationToken)
{
    // Set environment to Testing to disable authentication
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
    Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Testing");
    
    var appHost = await DistributedApplicationTestingBuilder
        .CreateAsync<Projects.DiscoverCostaRica_AppHost>(cancellationToken);

    appHost.Services.AddLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddSimpleConsole();
        logging.SetMinimumLevel(LogLevel.Debug);
    });

    appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
    {
        clientBuilder.AddStandardResilienceHandler();
    });

    // Add explicit configuration to disable EntraId authentication
    var testConfig = new Dictionary<string, string?>
    {
        ["EntraId:TenantId"] = "",
        ["EntraId:ClientId"] = "",
        // ... other EntraId config cleared
    };
    appHost.Configuration.AddInMemoryCollection(testConfig);

    return await appHost.BuildAsync(cancellationToken);
}
```

### Service Names in AppHost

| AppHost Resource Name | Service |
|----------------------|---------|
| `discovercostarica-beaches-api` | Beaches API |
| `discovercostarica-volcanoservice-api` | Volcano API |
| `discovercostarica-cultureserice-api` | Culture API |
| `discovercostarica-geoservice-api` | Geo API |
| `gateway` | YARP Gateway |
| `discovercostarica-functions` | Azure Functions |

## Benefits of These Tests

1. **Full Stack Validation**: Tests validate the entire distributed system, not just individual components
2. **Real Integration**: Uses actual services, databases, and message queues
3. **Aspire Best Practices**: Follows .NET Aspire testing patterns and recommendations
4. **Production-Like**: Tests run in an environment similar to production
5. **Fast Feedback**: Identifies integration issues early in development
6. **Documentation**: Tests serve as living documentation of the API
7. **Regression Prevention**: Prevents breaking changes to the API contracts

## Running Tests Locally

```bash
# Run all tests
dotnet test

# Run tests for a specific service
dotnet test --filter "FullyQualifiedName~BeachesIntegrationTest"

# Run with detailed output
dotnet test --verbosity detailed

# Run with test result export
dotnet test --logger "trx;LogFileName=test-results.trx"
```

## CI/CD Integration

These tests are designed for CI/CD pipelines:

```yaml
# GitHub Actions Example
- name: Run Integration Tests
  run: |
    dotnet test \
      --configuration Release \
      --logger "trx;LogFileName=test-results.trx" \
      --collect:"XPlat Code Coverage"

- name: Publish Test Results
  if: always()
  uses: dorny/test-reporter@v1
  with:
    name: Integration Tests
    path: '**/test-results.trx'
    reporter: dotnet-trx
```

## Future Enhancements

Potential improvements to the test suite:

1. **Performance Tests**: Add tests to measure response times
2. **Load Tests**: Test system behavior under load
3. **Chaos Engineering**: Test resilience by introducing failures
4. **Contract Tests**: Use Pact or similar for contract testing
5. **Security Tests**: Add security-focused integration tests
6. **Data Validation**: Validate response data structure and content
7. **Database State**: Add tests that validate database state changes
8. **Response Content Tests**: Add tests that validate actual API response content and structure

## Conclusion

This comprehensive test suite provides:
- ✅ 37 functional integration tests
- ✅ Coverage of all 4 microservices
- ✅ Gateway routing validation
- ✅ Aspire orchestration validation
- ✅ Health endpoint validation
- ✅ API endpoint validation
- ✅ Mock authentication support (bypasses Entra ID)
- ✅ Production-ready test patterns
- ✅ Production-ready test patterns

The tests ensure that the Discover Costa Rica application works correctly as a distributed system orchestrated by .NET Aspire.
