# Running Integration Tests - Guide

## Important: .NET Version Requirement

This project currently targets **.NET 10.0**, which is not yet released. To run the tests, you have two options:

### Option 1: Update to .NET 9.0 (Recommended for immediate testing)

If you want to run the tests immediately, you can temporarily downgrade to .NET 9.0:

1. Update all `.csproj` files that target `net10.0` to `net9.0`:

```bash
# Run this from the repository root
find . -name "*.csproj" -type f -exec sed -i 's/<TargetFramework>net10.0<\/TargetFramework>/<TargetFramework>net9.0<\/TargetFramework>/g' {} +
```

2. Verify the change:
```bash
dotnet --version  # Should show 9.x.x
```

3. Restore and build:
```bash
dotnet restore
dotnet build
```

4. Run tests:
```bash
dotnet test
```

### Option 2: Install .NET 10.0 Preview (When available)

Once .NET 10.0 preview is available:

1. Download from [https://dotnet.microsoft.com/download/dotnet/10.0](https://dotnet.microsoft.com/download/dotnet/10.0)
2. Install the SDK
3. Verify installation:
```bash
dotnet --version  # Should show 10.x.x
```

4. Run tests:
```bash
dotnet test
```

## Prerequisites

### Required
- .NET SDK (9.0 or 10.0)
- Docker Desktop (for running dependencies)
- Git

### Optional
- Visual Studio 2022 (17.8 or later)
- Visual Studio Code with C# extension
- JetBrains Rider 2024.1 or later

## Running Tests

### 1. Quick Test Run

Run all tests with default settings:

```bash
cd DiscoverCostaRica.Tests
dotnet test
```

### 2. Verbose Output

See detailed test execution information:

```bash
dotnet test --verbosity detailed
```

### 3. Run Specific Test Class

Run only tests from a specific class:

```bash
# Run only Beaches tests
dotnet test --filter "FullyQualifiedName~BeachesIntegrationTest"

# Run only Gateway tests
dotnet test --filter "FullyQualifiedName~GatewayIntegrationTest"

# Run only Aspire orchestration tests
dotnet test --filter "FullyQualifiedName~AspireOrchestrationTest"
```

### 4. Run Specific Test

Run a single test by name:

```bash
dotnet test --filter "FullyQualifiedName~BeachService_HealthEndpoint_ReturnsHealthy"
```

### 5. Generate Test Report

Export results to a TRX file:

```bash
dotnet test --logger "trx;LogFileName=test-results.trx"
```

### 6. Code Coverage

Generate code coverage report:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### 7. Run Tests in Parallel

By default, xUnit runs tests in parallel. To control this:

```bash
# Run serially (slower but easier to debug)
dotnet test -- xUnit.ParallelizeTestCollections=false

# Run with specific max threads
dotnet test -- xUnit.MaxParallelThreads=2
```

## Environment Setup

### Docker Services

The tests expect certain services to be available. Ensure Docker is running:

```bash
docker ps  # Should show Docker is running
```

### Configuration

Tests use the AppHost configuration. You may need to set up:

1. **Azure SQL Connection** (if not using connection string)
   - Set environment variables or user secrets
   - Or modify AppHost.cs to use local SQL Server

2. **MongoDB Connection** (for Functions)
   - Set environment variable: `ConnectionStrings__mongodb`
   - Or use Docker: `docker run -d -p 27017:27017 mongo`

3. **Azure Functions** (optional)
   - Tests can run without Azure Functions being fully functional
   - Functions may need additional Azure configuration

### User Secrets (Optional)

If you want to test with real Azure resources:

```bash
# Set SQL Server parameters
dotnet user-secrets set "Parameters:existingSqlServerName" "your-server-name"
dotnet user-secrets set "Parameters:existingSqlServerResourceGroup" "your-rg-name"

# Set MongoDB connection
dotnet user-secrets set "ConnectionStrings:mongodb" "your-mongodb-connection"
```

## Troubleshooting

### Issue: "The current .NET SDK does not support targeting .NET 10.0"

**Solution**: Follow Option 1 above to downgrade to .NET 9.0, or wait for .NET 10.0 to be released.

### Issue: Tests timeout

**Symptoms**: Tests fail with timeout errors

**Solutions**:
1. Increase timeout in test code (DefaultTimeout constant)
2. Ensure Docker has enough resources (4GB+ RAM)
3. Check if services are starting properly in Docker

```bash
# Check Docker resource usage
docker stats
```

### Issue: "Service not healthy"

**Symptoms**: Tests fail waiting for service to be healthy

**Solutions**:
1. Check Docker logs for the failing service
2. Verify database connection strings
3. Check for port conflicts
4. Ensure all dependencies are running

```bash
# View Aspire logs
dotnet run --project ../DiscoverCostaRica.AppHost
```

### Issue: Connection refused

**Symptoms**: Cannot connect to services

**Solutions**:
1. Verify Docker is running: `docker ps`
2. Check for port conflicts: `netstat -an | grep LISTEN`
3. Restart Docker Desktop
4. Clear Docker containers: `docker system prune -a`

### Issue: Authentication errors (401)

**Symptoms**: Tests return 401 Unauthorized

**This is expected!** The tests are designed to handle this:
- If Entra ID is configured, services will require authentication → 401 is valid
- If Entra ID is NOT configured, services will allow access → 200 is valid

The tests accept both responses as valid.

## Test Execution Times

Typical execution times (approximate):

- **Individual test**: 5-15 seconds
- **Test class**: 30-90 seconds
- **Full suite**: 3-5 minutes

First run may be slower as Docker images are pulled and services initialize.

## Running in Visual Studio

1. Open `DiscoverCostaRica.sln` in Visual Studio
2. Open Test Explorer (Test → Test Explorer)
3. Click "Run All Tests" or right-click specific tests

### Visual Studio Tips

- Use the search box in Test Explorer to filter tests
- Group by Project or Class for better organization
- Use "Debug Test" to debug failing tests

## Running in Visual Studio Code

1. Install the C# extension
2. Open the Test Explorer view (Testing icon in sidebar)
3. Click the play button to run tests

### VS Code Tips

- Use `.vscode/tasks.json` to create custom test tasks
- Use the integrated terminal to run dotnet test commands
- Install the "Coverage Gutters" extension to visualize code coverage

## Running in Rider

1. Open `DiscoverCostaRica.sln` in Rider
2. Open Unit Tests window (View → Tool Windows → Unit Tests)
3. Click "Run All" or right-click specific tests

### Rider Tips

- Use Ctrl+T,L to run tests in current file
- Use Ctrl+T,R to run all tests in solution
- Right-click and select "Cover All Tests" for coverage

## CI/CD Integration

### GitHub Actions

```yaml
name: Integration Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'  # or '10.0.x' when available
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Run tests
      run: dotnet test --no-build --verbosity normal --logger "trx"
    
    - name: Publish Test Results
      uses: dorny/test-reporter@v1
      if: always()
      with:
        name: Test Results
        path: '**/TestResults/*.trx'
        reporter: dotnet-trx
```

### Azure DevOps

```yaml
trigger:
  - main

pool:
  vmImage: 'ubuntu-latest'

steps:
- task: UseDotNet@2
  inputs:
    version: '9.x'  # or '10.x' when available

- script: dotnet restore
  displayName: 'Restore packages'

- script: dotnet build --no-restore
  displayName: 'Build solution'

- script: dotnet test --no-build --logger trx --collect:"XPlat Code Coverage"
  displayName: 'Run tests'

- task: PublishTestResults@2
  inputs:
    testResultsFormat: 'VSTest'
    testResultsFiles: '**/TestResults/*.trx'
```

## Best Practices

1. **Run tests before committing**: Ensure your changes don't break integration
2. **Run specific test classes during development**: Faster feedback loop
3. **Run full suite before PR**: Ensure all integrations work
4. **Keep Docker running**: Faster test execution
5. **Monitor resource usage**: Tests can be resource-intensive
6. **Review test output**: Understand why tests pass or fail

## Performance Tips

1. **Use SSD**: Faster Docker I/O
2. **Allocate enough RAM to Docker**: 4GB minimum, 8GB recommended
3. **Close unnecessary applications**: Free up resources
4. **Run tests in parallel**: Default behavior, but can be tuned
5. **Cache NuGet packages**: Speeds up restore operations

## Getting Help

If you encounter issues:

1. Check this guide's troubleshooting section
2. Review test output and logs
3. Check Docker logs: `docker logs <container-id>`
4. Review Aspire logs in the test output
5. Search existing issues on GitHub
6. Create a new issue with:
   - Steps to reproduce
   - Error messages
   - Environment details (OS, .NET version, Docker version)

## Additional Resources

- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Aspire Testing Guide](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/testing)
- [xUnit Documentation](https://xunit.net/)
- [Docker Documentation](https://docs.docker.com/)
