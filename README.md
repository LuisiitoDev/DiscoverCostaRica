# Discover Costa Rica

A modern microservices-based application built with .NET that provides information about Costa Rica's beaches, volcanoes, geographical locations, and cultural sites.

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Services](#services)
- [Technology Stack](#technology-stack)
- [Authentication & Authorization](#authentication--authorization)
- [Project Structure](#project-structure)
- [Contributors](#contributors)

## Overview

Discover Costa Rica is a distributed system built with **.NET Aspire**, following **Clean Architecture** principles. It uses **YARP** (integrated into AppHost) as an API Gateway, **Microsoft Entra ID** for authentication, **Dapr Client** for service-to-service communication, **RabbitMQ** for messaging, and microservices architecture for scalability.

## Architecture

### System Architecture

The application is orchestrated by .NET Aspire AppHost:

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                          .NET ASPIRE AppHost                                 │
│         (Orchestration, Service Discovery & YARP Gateway)                    │
│                         Port: 8080 (HTTPS)                                   │
└────────────────────────┬────────────────────────────────────────────────────┘
                         │
        ┌────────────────┼────────────────┬────────────────┐
        │                │                │                │
        ▼                ▼                ▼                ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│   Beach      │  │   Volcano    │  │   Culture    │  │     Geo      │
│   Service    │  │   Service    │  │   Service    │  │   Service    │
│              │  │              │  │              │  │              │
│ Minimal APIs │  │ Minimal APIs │  │ Minimal APIs │  │ Minimal APIs │
│ Entra ID +   │  │ Entra ID +   │  │ Entra ID +   │  │ Entra ID +   │
│ Dapr Client  │  │ Dapr Client  │  │ Dapr Client  │  │ Dapr Client  │
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
                                  │
                                  ▼
                        ┌──────────────────┐
                        │    RabbitMQ      │
                        │   (Messaging)    │
                        └──────────────────┘
                                  │
                                  ▼
                        ┌──────────────────┐
                        │  Azure Cosmos DB │
                        │  (MongoDB API)   │
                        │  • Logging &     │
                        │    State         │
                        └──────────────────┘

┌────────────────────────────────────────────────────────────────────────────┐
│                         Cross-Cutting Concerns                              │
│                                                                             │
│  • ServiceDefaults: OpenTelemetry, Health Checks, Service Discovery        │
│  • Shared Library: Authentication, Dapr Client, Custom Logging             │
│  • Source Generators: Automatic DI Registration                            │
│  • Microsoft Entra ID: JWT Bearer Token Authentication                     │
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
- **.NET Aspire**: Orchestration and service discovery with integrated YARP Gateway
- **Microsoft Entra ID**: JWT Bearer token authentication and authorization
- **Dapr Client**: Service-to-service communication using HTTP invocation
- **RabbitMQ**: Message broker for async communication
- **Shared Database**: Simplified microservices approach

## Services

### Microservices

1. **Beach Service** - Manages beach information and visitor details
2. **Volcano Service** - Handles volcano data and activity levels
3. **Culture Service** - Manages cultural sites and traditions
4. **Geo Service** - Handles geographical information and regions

### Infrastructure Components

5. **AppHost with YARP Gateway** - .NET Aspire orchestrator with integrated API Gateway for routing
6. **ServiceDefaults** - Shared configuration (OpenTelemetry, health checks, service discovery)
7. **Shared Library** - Authentication, Dapr Client wrapper, logging, API versioning, and utilities
8. **Source Generators** - Compile-time DI registration and authorization policies
9. **Tests** - Integration and unit tests

## Technology Stack

- **.NET 10.0** - Latest .NET framework
- **.NET Aspire 9.4/13.0** - Cloud-native orchestration with integrated YARP
- **ASP.NET Core Minimal APIs** - Lightweight API endpoints
- **Entity Framework Core 9.0** - ORM for data access
- **Azure SQL Database** - Primary relational database
- **Azure Cosmos DB (MongoDB API)** - NoSQL database for logging and state
- **Microsoft Entra ID (Azure AD)** - Authentication and authorization with JWT Bearer tokens
- **Dapr Client** - Service-to-service communication via HTTP method invocation
- **RabbitMQ** - Message broker for asynchronous communication
- **YARP (Yet Another Reverse Proxy)** - Integrated API Gateway for request routing
- **OpenTelemetry** - Distributed tracing and metrics
- **Asp.Versioning** - API versioning support
- **AutoMapper** - Object mapping
- **Refit** - Type-safe REST API client
- **Azure Event Grid** - Event-driven architecture support
- **Docker** - Containerization

## Authentication & Authorization

The application uses **Microsoft Entra ID (formerly Azure AD)** for secure authentication and authorization:

- **JWT Bearer Token Authentication** - All microservices validate JWT tokens issued by Entra ID
- **Role-Based Access Control (RBAC)** - Custom roles and policies enforce authorization rules
- **Scope-Based Authorization** - Fine-grained permissions using OAuth 2.0 scopes
- **Service-to-Service Auth** - Microservices authenticate with each other using Azure credentials
- **Source Generated Policies** - Authorization policies are automatically generated at compile-time

### Configuration

Each service is configured with Entra ID settings:
- **Instance**: Azure AD authority URL
- **TenantId**: Azure AD tenant identifier
- **ClientId**: Application registration ID
- **Audience**: Valid token audience for validation

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
├── DiscoverCostaRica.Shared/             # Shared libraries (authentication, Dapr client, logging, utils)
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
