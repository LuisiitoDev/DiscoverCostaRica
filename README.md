# Discover Costa Rica

A modern microservices-based application built with .NET that provides information about Costa Rica's beaches, volcanoes, geographical locations, and cultural sites.

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Services](#services)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Contributors](#contributors)

## Overview

Discover Costa Rica is a distributed system built with **.NET Aspire**, following **Clean Architecture** principles. It uses **Dapr** for service communication and state management, **YARP** as an API Gateway, and microservices architecture for scalability.

## Architecture

### System Architecture

The application is orchestrated by .NET Aspire AppHost:

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                          .NET ASPIRE AppHost                                 │
│                    (Orchestration & Service Discovery)                       │
└────────────────────────┬────────────────────────────────────────────────────┘
                         │
                         ▼
               ┌──────────────────┐
               │   YARP Gateway   │
               │  (API Gateway)   │
               │   Port: 9081     │
               └────────┬─────────┘
                        │
        ┌───────────────┼───────────────┬────────────────┐
        │               │               │                │
        ▼               ▼               ▼                ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│   Beach      │  │   Volcano    │  │   Culture    │  │     Geo      │
│   Service    │  │   Service    │  │   Service    │  │   Service    │
│              │  │              │  │              │  │              │
│ Minimal APIs │  │ Minimal APIs │  │ Minimal APIs │  │ Minimal APIs │
│   + Dapr     │  │   + Dapr     │  │   + Dapr     │  │   + Dapr     │
└──────┬───────┘  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘
       │                 │                 │                 │
       │ EF Core         │ EF Core         │ EF Core         │ EF Core
       ├─────────────────┼─────────────────┼─────────────────┤
       │                 │                 │                 │
       ▼                 ▼                 ▼                 ▼
┌─────────────────────────────────────────────────────────────────┐
│                    Azure SQL Database                            │
│               (Shared Database - Single Schema)                  │
│  • Beaches Table    • Volcanoes Table                           │
│  • Culture Table    • Geo Locations Table                       │
└─────────────────────────────────────────────────────────────────┘
       │                 │                 │                 │
       │   Dapr          │   Dapr          │   Dapr          │   Dapr
       │   Logger        │   Logger        │   Logger        │   Logger
       └─────────────────┴─────────────────┴─────────────────┘
                                  │
                                  ▼
                        ┌──────────────────┐
                        │  Dapr State      │
                        │  Store           │
                        └────────┬─────────┘
                                 │
                                 ▼
                       ┌───────────────────┐
                       │  Azure Cosmos DB  │
                       │  (MongoDB API)    │
                       │  • Application    │
                       │    Logs           │
                       └───────────────────┘

┌────────────────────────────────────────────────────────────────────────────┐
│                         Cross-Cutting Concerns                              │
│                                                                             │
│  • ServiceDefaults: OpenTelemetry, Health Checks, Service Discovery        │
│  • Shared Library: Dapr Client, Custom Logging, Response Models            │
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
- **Dapr**: Service communication and state management for logging
- **YARP Gateway**: API Gateway with request routing
- **Shared Database**: Simplified microservices approach

## Services

### Microservices

1. **Beach Service** - Manages beach information and visitor details
2. **Volcano Service** - Handles volcano data and activity levels
3. **Culture Service** - Manages cultural sites and traditions
4. **Geo Service** - Handles geographical information and regions

### Infrastructure Components

5. **YARP Gateway** - API Gateway for routing requests to microservices
6. **AppHost** - .NET Aspire orchestrator managing service discovery and dependencies
7. **ServiceDefaults** - Shared configuration (OpenTelemetry, health checks, service discovery)
8. **Shared Library** - Dapr client, logging, and utilities
9. **Source Generators** - Compile-time DI registration
10. **Tests** - Integration and unit tests

## Technology Stack

- **.NET 10.0** - Latest .NET framework
- **.NET Aspire 9.4** - Cloud-native orchestration
- **ASP.NET Core Minimal APIs** - Lightweight API endpoints
- **Entity Framework Core** - ORM for data access
- **Azure SQL Database** - Primary database
- **Azure Cosmos DB (MongoDB API)** - Log storage via Dapr state store
- **Dapr** - Distributed application runtime for service communication
- **YARP** - Reverse proxy for API Gateway
- **OpenTelemetry** - Distributed tracing and metrics
- **AutoMapper** - Object mapping
- **Docker** - Containerization

## Project Structure

```
DiscoverCostaRica/
├── src/
│   ├── DiscoverCostaRica.Beaches/        # Beach service (API, Application, Domain, Infrastructure)
│   ├── DiscoverCostaRica.Volcano/        # Volcano service
│   ├── DiscoverCostaRica.Culture/        # Culture service
│   └── DiscoverCostaRica.Geo/            # Geo service
├── DiscoverCostaRica.AppHost/            # Aspire orchestration + YARP Gateway
├── DiscoverCostaRica.ServiceDefaults/    # Shared service configuration
├── DiscoverCostaRica.Shared/             # Shared libraries (Dapr, logging, utils)
├── DiscoverCostaRica.SourceGenerators/   # Code generators
├── DiscoverCostaRica.Tests/              # Integration and unit tests
└── DiscoverCostaRica.sln
```

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
