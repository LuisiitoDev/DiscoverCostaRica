# Discover Costa Rica

A modern microservices-based application built with .NET that provides information about Costa Rica's beaches, volcanoes, geographical locations, and cultural sites.

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
  - [Complete System Architecture](#complete-system-architecture)
  - [Clean Architecture per Service](#clean-architecture-per-service)
  - [Key Architectural Patterns](#key-architectural-patterns)
- [Services](#services)
- [Technology Stack](#technology-stack)
- [Event-Driven Architecture](#event-driven-architecture)
- [Data Flow & Request Lifecycle](#data-flow--request-lifecycle)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Configuration](#configuration)
- [Development](#development)
- [API Documentation](#api-documentation)
- [Health Checks](#health-checks)
- [Monitoring & Observability](#monitoring--observability)
- [Deployment](#deployment)
- [Contributing](#contributing)
- [Contributors](#contributors)
- [License](#license)
- [Contact](#contact)

## Overview

Discover Costa Rica is a distributed system designed to showcase various aspects of Costa Rica's tourism destinations. The application follows Clean Architecture principles and leverages .NET Aspire for orchestration, Azure EventGrid for event-driven communication, and a microservices architecture for scalability and maintainability.

## Architecture

### Complete System Architecture

The Discover Costa Rica application is a distributed microservices system orchestrated by **.NET Aspire**. Here's the complete architecture:

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                          .NET ASPIRE AppHost                                 │
│                    (Orchestration & Service Discovery)                       │
└────────────────────────┬────────────────────────────────────────────────────┘
                         │
        ┌────────────────┼────────────────┬─────────────────┬─────────────────┐
        │                │                │                 │                 │
        ▼                ▼                ▼                 ▼                 ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  ┌────────────────┐
│   Beach      │  │   Volcano    │  │   Culture    │  │     Geo      │  │ Log Consumer   │
│   Service    │  │   Service    │  │   Service    │  │   Service    │  │   Function     │
│              │  │              │  │              │  │              │  │  (Azure Func)  │
│ Minimal APIs │  │ Minimal APIs │  │ Minimal APIs │  │ Minimal APIs │  │                │
└──────┬───────┘  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘  └───────┬────────┘
       │                 │                 │                 │                   │
       │ EF Core         │ EF Core         │ EF Core         │ EF Core           │
       ├─────────────────┼─────────────────┼─────────────────┤                   │
       │                 │                 │                 │                   │
       ▼                 ▼                 ▼                 ▼                   │
┌─────────────────────────────────────────────────────────────────┐             │
│                    SQL Server Database                           │             │
│               (Shared Database - Single Schema)                  │             │
│  • Beaches Table    • Volcanoes Table                           │             │
│  • Culture Table    • Geo Locations Table                       │             │
└─────────────────────────────────────────────────────────────────┘             │
       │                 │                 │                 │                   │
       │                 │                 │                 │                   │
       │   EventGrid     │   EventGrid     │   EventGrid     │   EventGrid       │
       │   Logger        │   Logger        │   Logger        │   Logger          │
       └─────────────────┴─────────────────┴─────────────────┴───────────────────┤
                                           │                                      │
                                           ▼                                      │
                                  ┌──────────────────┐                           │
                                  │  Azure EventGrid │                           │
                                  │   Topic/Events   │                           │
                                  └────────┬─────────┘                           │
                                           │                                      │
                                           └──────────────────────────────────────┘
                                                          │
                                                          ▼
                                                ┌───────────────────┐
                                                │  Azure Cosmos DB  │
                                                │  (Log Storage)    │
                                                │  • Application    │
                                                │    Logs           │
                                                │  • Telemetry      │
                                                └───────────────────┘

┌────────────────────────────────────────────────────────────────────────────┐
│                         Cross-Cutting Concerns                              │
│                                                                             │
│  • ServiceDefaults: OpenTelemetry, Health Checks, Service Discovery        │
│  • Shared Library: EventGrid Client, Custom Logging, Response Models       │
│  • Source Generators: Automatic DI Registration                            │
└────────────────────────────────────────────────────────────────────────────┘
```

### Clean Architecture per Service

Each microservice follows **Clean Architecture** principles with a clear separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│                      API Layer                               │
│           (Minimal API Endpoints, Controllers)               │
│  • Routing • Validation • DTO Mapping • HTTP Handling       │
└────────────────────┬────────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────────┐
│                    Application Layer                         │
│              (Business Logic, Use Cases)                     │
│  • Services • DTOs • Interfaces • AutoMapper Profiles       │
└────────────────────┬────────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────────┐
│                      Domain Layer                            │
│            (Entities, Domain Logic, Rules)                   │
│  • Entities • Domain Models • Business Rules • Interfaces   │
└────────────────────┬────────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────────┐
│                  Infrastructure Layer                        │
│         (Data Access, External Services)                     │
│  • EF Core DbContext • Repositories • EventGrid Client      │
│  • Entity Configurations • Database Migrations              │
└─────────────────────────────────────────────────────────────┘
```

### Key Architectural Patterns

- **Clean Architecture**: Each service is organized into Domain, Application, Infrastructure, and API layers with strict dependency rules (dependencies point inward)
- **Microservices**: Independent, loosely-coupled services for different business domains (Beaches, Volcanoes, Culture, Geo)
- **.NET Aspire Orchestration**: Centralized service orchestration, discovery, and configuration management
- **Event-Driven Architecture**: Services communicate asynchronously via Azure EventGrid for logging and domain events
- **Shared Database**: All services share a single SQL Server database (different from typical microservices, simplified for this project)
- **CQRS (Command Query Responsibility Segregation)**: Separation of read and write operations within services
- **Dependency Injection**: Built-in .NET DI container for loose coupling and testability
- **Source Generators**: Custom Roslyn source generators for compile-time service registration
- **Repository Pattern**: Abstraction over data access logic
- **API Versioning**: Version management for backward compatibility

## Services

### 1. Beach Service (`DiscoverCostaRica.Beaches`)
Manages information about Costa Rica's beaches, including location, characteristics, and visitor information.

**Endpoints:**
- GET `/api/v1/beaches` - List all beaches
- GET `/api/v1/beaches/{id}` - Get beach details
- POST `/api/v1/beaches` - Create beach (admin)
- PUT `/api/v1/beaches/{id}` - Update beach (admin)
- DELETE `/api/v1/beaches/{id}` - Delete beach (admin)

### 2. Volcano Service (`DiscoverCostaRica.Volcano`)
Provides details about Costa Rica's volcanoes, their activity levels, and tourist accessibility.

**Endpoints:**
- GET `/api/v1/volcanoes` - List all volcanoes
- GET `/api/v1/volcanoes/{id}` - Get volcano details
- POST `/api/v1/volcanoes` - Create volcano (admin)
- PUT `/api/v1/volcanoes/{id}` - Update volcano (admin)
- DELETE `/api/v1/volcanoes/{id}` - Delete volcano (admin)

### 3. Culture Service (`DiscoverCostaRica.Culture`)
Manages cultural sites, traditions, and historical locations throughout Costa Rica.

**Endpoints:**
- GET `/api/v1/culture` - List all cultural sites
- GET `/api/v1/culture/{id}` - Get cultural site details
- POST `/api/v1/culture` - Create cultural site (admin)
- PUT `/api/v1/culture/{id}` - Update cultural site (admin)
- DELETE `/api/v1/culture/{id}` - Delete cultural site (admin)

### 4. Geo Service (`DiscoverCostaRica.Geo`)
Handles geographical information, regions, and location-based queries.

**Endpoints:**
- GET `/api/v1/geo` - List all geographical locations
- GET `/api/v1/geo/{id}` - Get location details
- POST `/api/v1/geo` - Create location (admin)
- PUT `/api/v1/geo/{id}` - Update location (admin)
- DELETE `/api/v1/geo/{id}` - Delete location (admin)

### 5. Log Consumer Function (`DiscoverCostaRica.Function.LogConsumer`)
Azure Function that consumes log events from EventGrid and persists them to Azure Cosmos DB for centralized logging and monitoring.

**Trigger:**
- EventGrid Trigger: Subscribes to log events from all services

**Output:**
- Writes structured logs to Azure Cosmos DB for analysis and monitoring

### 6. AppHost Orchestration (`DiscoverCostaRica.AppHost`)

The **AppHost** is the heart of the .NET Aspire orchestration layer that manages all services, dependencies, and infrastructure components.

**Responsibilities:**
- **Service Discovery**: Automatic registration and discovery of all microservices
- **Dependency Management**: Orchestrates startup order with `WaitFor()` dependencies
- **Configuration Distribution**: Centralizes connection strings and configuration
- **Local Development**: Provides development dashboard at `http://localhost:15888`
- **Infrastructure Provisioning**: Sets up SQL Server, Cosmos DB, and EventGrid connections

**Managed Resources:**
```csharp
// SQL Server with persistent data
var sql = builder.AddSqlServer("sql").WithLifetime(ContainerLifetime.Persistent);
var db = sql.AddDatabase("DiscoverCostaRica");

// Azure Cosmos DB for logging
var cosmos = builder.AddAzureCosmosDB("discovercostarica-db");
var discoverCostaRicaLogger = cosmos.AddCosmosDatabase("discovercostarica-logger");

// All services with database dependency
builder.AddProject<Projects.DiscoverCostaRica_Beaches_Api>("beaches-api")
       .WithReference(db)
       .WaitFor(db);

// Azure Function with Cosmos DB reference
builder.AddAzureFunctionsProject<Projects.DiscoverCostaRica_Function_LogConsumer>("log-consumer")
       .WithReference(discoverCostaRicaLogger);
```

### 7. Service Defaults (`DiscoverCostaRica.ServiceDefaults`)

Shared configuration library that every service references for consistent behavior:

**Provides:**
- **OpenTelemetry Integration**: Automatic tracing, metrics, and logging
- **Health Checks**: `/health` and `/alive` endpoints for monitoring
- **Service Discovery Client**: Enables services to discover each other
- **Resilience Patterns**: HTTP retry policies and circuit breakers
- **EventGrid Logger**: Custom logger provider that publishes to EventGrid
- **Global Exception Handling**: Consistent error responses across all services
- **Database Context Registration**: Helper methods for EF Core setup

### 8. Shared Library (`DiscoverCostaRica.Shared`)

Common components used across all services:

**Components:**
- **EventGrid Client**: Reusable client for publishing events to Azure EventGrid
- **EventGrid Logger**: Custom logger that sends application logs to EventGrid → Cosmos DB
- **Response Models**: Standardized API response structures
- **Utility Functions**: Common helper methods
- **DI Attributes**: `[ScopedService]`, `[SingletonService]`, `[TransientService]` for automatic registration

### 9. Source Generators (`DiscoverCostaRica.SourceGenerators`)

Custom Roslyn source generators that run at compile-time:

**Purpose:**
- Automatically generates service registration code based on custom attributes
- Eliminates boilerplate DI registration code
- Ensures consistency across all services
- Generates methods like `AddGeneratedServices_DiscoverCostaRica_Beaches_Application()`

**Example:**
```csharp
[ScopedService]
public class BeachService : IBeachService
{
    // Implementation
}

// Generated code at compile-time:
public static IServiceCollection AddGeneratedServices_DiscoverCostaRica_Beaches_Application(
    this IServiceCollection services)
{
    services.AddScoped<IBeachService, BeachService>();
    return services;
}
```

## Technology Stack

### Core Technologies
- **.NET 10.0**: Latest .NET framework for high-performance APIs
- **.NET Aspire 9.4**: Cloud-native orchestration and service management
- **C# 13**: Modern C# with latest language features

### Data Storage
- **SQL Server**: Primary relational database for transactional data
- **Azure Cosmos DB**: NoSQL database for log storage and analytics
- **Entity Framework Core**: ORM for data access

### Messaging & Events
- **Azure EventGrid**: Event-driven messaging for inter-service communication
- **EventGrid SDK**: Azure.Messaging.EventGrid for event publishing and consumption

### API & Web
- **ASP.NET Core Minimal APIs**: Lightweight, high-performance API endpoints
- **Swagger/OpenAPI**: API documentation and testing
- **API Versioning**: Version management with Asp.Versioning

### Monitoring & Observability
- **OpenTelemetry**: Distributed tracing and metrics
- **Application Insights**: (Optional) Azure monitoring integration
- **Health Checks**: Built-in health monitoring endpoints

### Development Tools
- **AutoMapper**: Object-to-object mapping
- **Roslyn Source Generators**: Compile-time code generation for DI registration
- **Docker**: Containerization support

### Cloud Infrastructure
- **Azure Functions**: Serverless compute for event processing
- **Azure EventGrid Topics**: Event routing and distribution
- **Service Discovery**: Built-in .NET Aspire service discovery

## Event-Driven Architecture

The application uses **Azure EventGrid** for asynchronous, event-driven communication between services. This replaces the previous RabbitMQ implementation with a more scalable, cloud-native solution.

### EventGrid Implementation

#### Event Publishing
Services publish events to EventGrid when significant actions occur:

```csharp
public class EventGridClient : IEventGridClient
{
    public void PublishEvent(EventGridEvent message)
    {
        var credential = new AzureKeyCredential(topicKey);
        var client = new EventGridPublisherClient(topicEndpoint, credential);
        await client.SendEventAsync(message);
    }
}
```

#### Event Consumption
The Log Consumer Function subscribes to EventGrid events:

```csharp
[FunctionName("LogConsumerFunction")]
public static void Run([EventGridTrigger] EventGridEvent eventGridEvent, IMongoLogger log)
{
    var content = JsonSerializer.Deserialize<LogEntryModel>(eventGridEvent.Data.ToArray());
    log.Log(content);
}
```

### Event Types
- **Log Events**: Application logs, errors, and diagnostic information
- **Domain Events**: Business events (beach created, volcano updated, etc.)

### EventGrid Configuration

Each service requires EventGrid configuration in `appsettings.json`:

```json
{
  "EventGridOptions": {
    "TopicEndpoint": "https://<topic-name>.<region>.eventgrid.azure.net/api/events",
    "TopicKey": "<access-key>"
  }
}
```

## Data Flow & Request Lifecycle

### Typical Request Flow

Here's how a typical API request flows through the architecture:

```
1. Client Request
   │
   ├──→ API Layer (Minimal API Endpoint)
   │    • Request validation
   │    • Route matching
   │    • Authentication/Authorization
   │
   ├──→ Application Layer (Service)
   │    • Business logic execution
   │    • DTO mapping (AutoMapper)
   │    • Use case orchestration
   │
   ├──→ Domain Layer (Entities & Rules)
   │    • Domain validation
   │    • Business rule enforcement
   │    • Domain events creation
   │
   ├──→ Infrastructure Layer (Repository)
   │    • EF Core DbContext
   │    • Database query/command
   │    • Data persistence
   │
   └──→ Response
        • DTO mapping
        • HTTP response formatting
        • Status code selection

During Execution:
   ├──→ EventGrid Logger
   │    • Log events published to EventGrid
   │    • Azure Function consumes logs
   │    • Logs stored in Cosmos DB
   │
   └──→ OpenTelemetry
        • Trace context propagation
        • Span creation
        • Metrics collection
```

### Example: Create Beach Request

```
POST /api/v1/beaches
Body: { "name": "Tamarindo", "region": "Guanacaste", ... }

Flow:
1. BeachesApi/Endpoints receives request
2. Maps request to CreateBeachCommand DTO
3. Calls IBeachService.CreateBeachAsync()
4. Service validates business rules (Domain layer)
5. Creates Beach entity
6. Repository saves to SQL Server via EF Core
7. EventGrid Logger publishes "BeachCreated" log event
8. Response mapped to BeachDto
9. Returns 201 Created with location header

Parallel:
- EventGrid → Log Consumer Function → Cosmos DB (async)
- OpenTelemetry traces the entire request chain
- Health checks monitor service availability
```

### Service Communication Patterns

**Current Implementation:**
- **Shared Database**: All services read/write to the same SQL Server database
- **Independent Schemas**: Each service manages its own tables
- **EventGrid for Logging**: Centralized logging via EventGrid → Cosmos DB
- **Service Discovery**: Services discover each other via .NET Aspire

**Future Considerations:**
- Could evolve to **Database per Service** pattern for true isolation
- Could add **API Gateway** for unified entry point
- Could implement **Saga pattern** for distributed transactions
- Could add **Domain Events** via EventGrid for cross-service communication

## Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for local development)
- [Azure Subscription](https://azure.microsoft.com/) (for EventGrid and Cosmos DB)
- [Visual Studio 2025](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)

### Local Development Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/LuisiitoDev/DiscoverCostaRica.git
   cd DiscoverCostaRica
   ```

2. **Configure Azure Resources**
   
   Create the following Azure resources:
   - Azure EventGrid Topic
   - Azure Cosmos DB account
   - SQL Server database (or use local Docker container)

3. **Update Configuration**
   
   Update `appsettings.Development.json` in each service with your connection strings:
   ```json
   {
     "ConnectionStrings": {
       "DiscoverCostaRica": "Server=localhost;Database=DiscoverCostaRica;..."
     },
     "EventGridOptions": {
       "TopicEndpoint": "your-eventgrid-topic-endpoint",
       "TopicKey": "your-eventgrid-topic-key"
     }
   }
   ```

4. **Run the Application**
   
   Using .NET Aspire AppHost (Recommended):
   ```bash
   cd DiscoverCostaRica.AppHost
   dotnet run
   ```
   
   This will:
   - Start all 4 microservices (Beaches, Volcano, Culture, Geo)
   - Launch SQL Server container (with persistent storage)
   - Initialize the Azure Function (Log Consumer)
   - Configure service discovery and dependencies
   - Open the Aspire Dashboard at `http://localhost:15888`

5. **Access the Aspire Dashboard**
   
   Navigate to `http://localhost:15888` to see:
   - **Resources**: All running services and their status
   - **Console Logs**: Real-time logs from all services
   - **Traces**: Distributed traces across services (OpenTelemetry)
   - **Metrics**: Performance metrics and health status
   - **Environment Variables**: Configuration for each service

6. **Access Services**
   
   Once all services are running (check Aspire Dashboard):
   - Beach API: `https://localhost:7001` or check dashboard for dynamic port
   - Volcano API: `https://localhost:7002` or check dashboard for dynamic port
   - Culture API: `https://localhost:7003` or check dashboard for dynamic port
   - Geo API: `https://localhost:7004` or check dashboard for dynamic port
   - Swagger UI: Available at each service's `/swagger` endpoint

### Understanding the Startup Flow

1. **AppHost starts** → Reads `AppHost.cs` configuration
2. **SQL Server container launches** → Persistent database initialized
3. **Each microservice starts** → Waits for database to be ready (`WaitFor(db)`)
4. **Services register** → Auto-discovered via .NET Aspire service discovery
5. **EventGrid Logger activates** → All logs route to EventGrid → Cosmos DB
6. **Health checks** → Services report status to Aspire Dashboard
7. **OpenTelemetry** → Distributed tracing and metrics collection begins

### Running Individual Services

To run a service independently:

```bash
cd src/DiscoverCostaRica.Beaches/DiscoverCostaRica.Beaches.Api
dotnet run
```

## Project Structure

```
DiscoverCostaRica/
├── src/
│   ├── DiscoverCostaRica.Beaches/
│   │   ├── DiscoverCostaRica.Beaches.Api/          # API endpoints
│   │   ├── DiscoverCostaRica.Beaches.Application/  # Business logic
│   │   ├── DiscoverCostaRica.Beaches.Domain/       # Domain models
│   │   └── DiscoverCostaRica.Beaches.Infrastructure/ # Data access
│   ├── DiscoverCostaRica.Volcano/
│   │   ├── DiscoverCostaRica.VolcanoService.Api/
│   │   ├── DiscoverCostaRica.VolcanoService.Application/
│   │   ├── DiscoverCostaRica.VolcanoService.Domain/
│   │   └── DiscoverCostaRica.VolcanoService.Infrastructure/
│   ├── DiscoverCostaRica.Culture/
│   │   ├── DiscoverCostaRica.Culture.Api/
│   │   ├── DiscoverCostaRica.Culture.Application/
│   │   ├── DiscoverCostaRica.Culture.Domain/
│   │   └── DiscoverCostaRica.Culture.Infrastructure/
│   └── DiscoverCostaRica.Geo/
│       ├── DiscoverCostaRica.Geo.Api/
│       ├── DiscoverCostaRica.Geo.Application/
│       ├── DiscoverCostaRica.Geo.Domain/
│       └── DiscoverCostaRica.Geo.Infrastructure/
├── DiscoverCostaRica.AppHost/                      # Aspire orchestration
├── DiscoverCostaRica.ServiceDefaults/              # Shared service configuration
├── DiscoverCostaRica.Shared/                       # Shared libraries
│   ├── EventGrid/                                  # EventGrid client
│   ├── Logging/                                    # Custom logging
│   ├── Responses/                                  # Common response models
│   └── Utils/                                      # Utility functions
├── DiscoverCostaRica.SourceGenerators/             # Code generators
├── DiscoverCostaRica.Function.LogConsumer/         # Azure Function for logging
└── DiscoverCostaRica.sln                           # Solution file
```

### Layer Responsibilities

#### API Layer
- Minimal API endpoints
- HTTP request/response handling
- API versioning
- Swagger/OpenAPI documentation
- Request validation

#### Application Layer
- Business logic implementation
- DTOs (Data Transfer Objects)
- Service interfaces and implementations
- AutoMapper profiles
- Application-specific exceptions

#### Domain Layer
- Domain entities
- Business rules
- Domain interfaces
- Value objects
- Domain events

#### Infrastructure Layer
- Entity Framework DbContext
- Repository implementations
- External service integrations
- Data persistence
- EventGrid client usage

## Configuration

### Service Defaults (`DiscoverCostaRica.ServiceDefaults`)

The ServiceDefaults project provides common configuration for all services:

- **OpenTelemetry**: Distributed tracing and metrics
- **Health Checks**: `/health` and `/alive` endpoints
- **Service Discovery**: Automatic service registration and discovery
- **Resilience**: HTTP client retry policies
- **EventGrid Integration**: Centralized EventGrid logger configuration
- **Global Exception Handling**: Consistent error responses

### Shared Components (`DiscoverCostaRica.Shared`)

#### EventGrid Client
Provides a reusable client for publishing events to Azure EventGrid.

#### Custom Logging
EventGrid-based logger that sends application logs to Cosmos DB via EventGrid.

#### Attributes for Dependency Injection
- `[ScopedService]`: Registers as scoped service
- `[SingletonService]`: Registers as singleton service
- `[TransientService]`: Registers as transient service

These attributes are processed by Roslyn Source Generators to automatically register services in the DI container.

### Source Generators

The project uses custom Roslyn source generators (`DiscoverCostaRica.SourceGenerators`) to automatically generate service registration code at compile-time, reducing boilerplate and preventing registration errors.

## Development

### Building the Solution

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

### Database Migrations

Each service manages its own database schema. To create a migration:

```bash
cd src/DiscoverCostaRica.Beaches/DiscoverCostaRica.Beaches.Infrastructure
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Adding a New Service

1. Create service projects following the Clean Architecture structure
2. Add domain entities in the Domain layer
3. Implement business logic in the Application layer
4. Create repository implementations in the Infrastructure layer
5. Define API endpoints in the API layer
6. Register the service in `AppHost.cs`
7. Configure dependencies in `Program.cs`

### Code Style

- Follow C# coding conventions
- Use nullable reference types
- Implement proper error handling
- Document public APIs with XML comments
- Use async/await for I/O operations
- Follow SOLID principles

## API Documentation

Each service exposes Swagger/OpenAPI documentation in development mode:

- Beach API: `https://localhost:7001/swagger`
- Volcano API: `https://localhost:7002/swagger`
- Culture API: `https://localhost:7003/swagger`
- Geo API: `https://localhost:7004/swagger`

## Health Checks

All services implement health check endpoints:

- `/health` - Complete health check (database, dependencies)
- `/alive` - Basic liveness check

## Monitoring & Observability

The application uses OpenTelemetry for comprehensive observability:

- **Traces**: Request tracing across services
- **Metrics**: Performance counters and custom metrics
- **Logs**: Structured logging with EventGrid integration

Configure OTLP exporter in `appsettings.json`:

```json
{
  "OTEL_EXPORTER_OTLP_ENDPOINT": "https://your-otlp-endpoint"
}
```

## Deployment

### Docker

Each API service includes a Dockerfile for containerization:

```bash
docker build -t discovercostarica-beaches:latest -f src/DiscoverCostaRica.Beaches/DiscoverCostaRica.Beaches.Api/Dockerfile .
```

### Azure Deployment

The application is designed for Azure deployment:

1. **Azure App Services**: Host API services
2. **Azure Functions**: Run the Log Consumer function
3. **Azure SQL Database**: Production database
4. **Azure Cosmos DB**: Log storage
5. **Azure EventGrid**: Event messaging
6. **Azure Container Registry**: Store Docker images

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## Contributors

We appreciate all the contributors who have helped make this project better!

### Core Team
- **Luis (LuisiitoDev)** - [@LuisiitoDev](https://github.com/LuisiitoDev) - Project Creator & Lead Developer

### How to Contribute

Contributions are welcome! Please see the [Contributing](#contributing) section above for guidelines on how to submit your changes.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contact

- **Project Maintainer**: Luis (LuisiitoDev)
- **Repository**: [https://github.com/LuisiitoDev/DiscoverCostaRica](https://github.com/LuisiitoDev/DiscoverCostaRica)
