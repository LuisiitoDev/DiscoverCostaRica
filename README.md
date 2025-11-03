# Discover Costa Rica

A modern microservices-based application built with .NET that provides information about Costa Rica's beaches, volcanoes, geographical locations, and cultural sites.

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Services](#services)
- [Technology Stack](#technology-stack)
- [Getting Started](#getting-started)
- [Development](#development)
- [Contributors](#contributors)

## Overview

Discover Costa Rica is a distributed system built with **.NET Aspire**, following **Clean Architecture** principles. It uses **Azure EventGrid** for event-driven communication and microservices architecture for scalability.

## Architecture

### System Architecture

The application is orchestrated by .NET Aspire AppHost:

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

### Clean Architecture Layers

Each service follows Clean Architecture with four layers:

```
┌─────────────────────────────────────────────────────────────┐
│                      API Layer                               │
│           (Minimal API Endpoints, Controllers)               │
└────────────────────┬────────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────────┐
│                    Application Layer                         │
│              (Business Logic, Use Cases)                     │
└────────────────────┬────────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────────┐
│                      Domain Layer                            │
│            (Entities, Domain Logic, Rules)                   │
└────────────────────┬────────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────────┐
│                  Infrastructure Layer                        │
│         (Data Access, External Services)                     │
└─────────────────────────────────────────────────────────────┘
```

### Key Patterns

- **Clean Architecture**: Dependency rules pointing inward
- **Microservices**: Independent services per domain
- **.NET Aspire**: Orchestration and service discovery
- **Event-Driven**: Azure EventGrid for logging
- **Shared Database**: Simplified microservices approach

## Services

### Microservices

1. **Beach Service** - Manages beach information and visitor details
2. **Volcano Service** - Handles volcano data and activity levels
3. **Culture Service** - Manages cultural sites and traditions
4. **Geo Service** - Handles geographical information and regions

### Infrastructure Components

5. **Log Consumer Function** - Azure Function that consumes EventGrid logs and stores them in Cosmos DB
6. **AppHost** - .NET Aspire orchestrator managing service discovery and dependencies
7. **ServiceDefaults** - Shared configuration (OpenTelemetry, health checks, service discovery)
8. **Shared Library** - EventGrid client, logging, and utilities
9. **Source Generators** - Compile-time DI registration

## Technology Stack

- **.NET 10.0** - Latest .NET framework
- **.NET Aspire 9.4** - Cloud-native orchestration
- **ASP.NET Core Minimal APIs** - Lightweight API endpoints
- **Entity Framework Core** - ORM for data access
- **SQL Server** - Primary database
- **Azure Cosmos DB** - Log storage
- **Azure EventGrid** - Event messaging
- **OpenTelemetry** - Distributed tracing and metrics
- **AutoMapper** - Object mapping
- **Docker** - Containerization

## Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Azure Subscription](https://azure.microsoft.com/) (for EventGrid and Cosmos DB)

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/LuisiitoDev/DiscoverCostaRica.git
   cd DiscoverCostaRica
   ```

2. **Configure Azure Resources**
   
   Create Azure EventGrid Topic and Cosmos DB account.

3. **Update Configuration**
   
   Add connection strings in `appsettings.Development.json`:
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

4. **Run with .NET Aspire**
   ```bash
   cd DiscoverCostaRica.AppHost
   dotnet run
   ```
   
   Access the Aspire Dashboard at `http://localhost:15888`

### Project Structure

```
DiscoverCostaRica/
├── src/
│   ├── DiscoverCostaRica.Beaches/        # Beach service (API, Application, Domain, Infrastructure)
│   ├── DiscoverCostaRica.Volcano/        # Volcano service
│   ├── DiscoverCostaRica.Culture/        # Culture service
│   └── DiscoverCostaRica.Geo/            # Geo service
├── DiscoverCostaRica.AppHost/            # Aspire orchestration
├── DiscoverCostaRica.ServiceDefaults/    # Shared service configuration
├── DiscoverCostaRica.Shared/             # Shared libraries
├── DiscoverCostaRica.SourceGenerators/   # Code generators
├── DiscoverCostaRica.Function.LogConsumer/ # Azure Function
└── DiscoverCostaRica.sln
```

## Development

### Building

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

### Database Migrations

```bash
cd src/DiscoverCostaRica.Beaches/DiscoverCostaRica.Beaches.Infrastructure
dotnet ef migrations add MigrationName
dotnet ef database update
```

### API Documentation

Each service exposes Swagger/OpenAPI documentation at `/swagger` endpoint in development mode.

### Health Checks

All services implement:
- `/health` - Complete health check
- `/alive` - Basic liveness check

## Contributors

### Core Team
- **Luis (LuisiitoDev)** - [@LuisiitoDev](https://github.com/LuisiitoDev) - Project Creator & Lead Developer

### Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contact

- **Project Maintainer**: Luis (LuisiitoDev)
- **Repository**: [https://github.com/LuisiitoDev/DiscoverCostaRica](https://github.com/LuisiitoDev/DiscoverCostaRica)
