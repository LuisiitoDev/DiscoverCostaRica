# Discover Costa Rica

A tourism information platform built as a set of RESTful microservices on **.NET 10**, providing data about Costa Rica's beaches, volcanoes, geographical locations, and cultural sites.

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Services](#services)
- [Technology Stack](#technology-stack)
- [Authentication & Authorization](#authentication--authorization)
- [Caching](#caching)
- [Observability](#observability)
- [API Versioning](#api-versioning)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Contributors](#contributors)

## Overview

Discover Costa Rica is a distributed system built with **.NET Aspire** as the local development orchestrator and **Azure Container Apps** as the production target. It follows **Clean Architecture** principles within each microservice, uses **YARP** as an API Gateway, **Microsoft Entra ID** for authentication, and **Redis** for distributed caching.

## Architecture

### System Context

```
┌─────────────────────────────────────────────────────────┐
│                       CLIENTS                           │
│   (Browsers, Mobile Apps, Other Services)               │
└────────────────────────┬────────────────────────────────┘
                         │ HTTPS
                         ▼
┌────────────────────────────────────────────────────────┐
│              YARP API GATEWAY (gateway)                │
│   Routes and transforms public routes to services      │
└──────┬──────────┬──────────┬──────────┬────────────────┘
       │          │          │          │
       ▼          ▼          ▼          ▼
  ┌─────────┐ ┌──────────┐ ┌────────┐ ┌──────────────┐
  │ Beaches │ │ Culture  │ │  Geo   │ │   Volcano    │
  │  API    │ │   API    │ │  API   │ │    API       │
  └────┬────┘ └────┬─────┘ └───┬────┘ └──────┬───────┘
       │           │           │              │
       └───────────┴───────────┴──────────────┘
                         │
              ┌──────────┴──────────────┐
              │                         │
     ┌────────▼────────┐    ┌──────────▼──────────┐
     │  Azure SQL DB   │    │  Redis Cache         │
     │  (shared)       │    │  (distributed cache) │
     └─────────────────┘    └──────────────────────┘
              │
     ┌────────▼────────┐
     │  MongoDB        │
     │  (logs)         │
     └─────────────────┘
```

### Clean Architecture Layers

Each microservice is divided into four projects following the Clean Architecture dependency rules:

```
┌────────────────────────────────────────────────────────┐
│                     API Layer                          │
│  Program.cs · EndpointExtensions · Handlers · Profiles │
│            (Minimal API, AutoMapper, OpenAPI)           │
└──────────────────────────┬─────────────────────────────┘
                           │ depends on
┌──────────────────────────▼─────────────────────────────┐
│                 Application Layer                       │
│         DTOs · Interfaces · Services · Cache            │
│          (Business logic, no external dependencies)     │
└──────────────────────────┬─────────────────────────────┘
                           │ depends on
┌──────────────────────────▼─────────────────────────────┐
│                   Domain Layer                          │
│              Models · Repository Interfaces             │
│              (Pure entities, no dependencies)           │
└─────────────────────────────────────────────────────────┘
                           ▲ implemented by
┌──────────────────────────┴─────────────────────────────┐
│                Infrastructure Layer                     │
│   DbContext · EntityConfigurations · Repositories       │
│   (EF Core, SQL Server, concrete data access)           │
└─────────────────────────────────────────────────────────┘
```

### Key Patterns

- **Clean Architecture**: Dependency rules pointing inward
- **Microservices**: Independent services per domain
- **.NET Aspire**: Local orchestration and service discovery with integrated YARP Gateway
- **Microsoft Entra ID**: JWT Bearer token authentication and authorization
- **Shared Database**: Single Azure SQL database with schema separation per service
- **Redis Distributed Cache**: Decorator pattern wraps services transparently
- **Source Generators**: Compile-time DI registration and authorization policy generation
- **Resilience**: Retries, circuit breaker, and timeouts via `AddStandardResilienceHandler()`

## Services

### Microservices

| Service | Gateway Prefix | Internal Base URL | Authorization Policy |
|---|---|---|---|
| **Beaches API** | `/beaches/` | `/api/v1/beaches/` | `Beaches.Read` |
| **Culture API** | `/tradition/`, `/dish/` | `/api/v1/traditions/` | `Culture.Read` |
| **Geo API** | `/provinces/`, `/canton/`, `/districts/` | `/api/v1/geo/` | `Geo.Read` |
| **Volcano API** | `/volcano/`, `/province/` | `/api/v1/volcanoes/` | `Volcano.Read` |

#### Beaches API

| Method | Route | Description |
|---|---|---|
| GET | `/api/v1/beaches/` | List all beaches |

#### Culture API

| Method | Route | Description |
|---|---|---|
| GET | `/api/v1/traditions/dish` | List typical dishes |
| GET | `/api/v1/traditions/tradition` | List traditions |

#### Geo API

| Method | Route | Description |
|---|---|---|
| GET | `/api/v1/geo/provinces` | List provinces |
| GET | `/api/v1/geo/provinces/{provinceId}` | Province by ID |
| GET | `/api/v1/geo/cantons/{provinceId}` | Cantons of a province |
| GET | `/api/v1/geo/cantons/{provinceId}/{cantonId}` | Canton by ID |
| GET | `/api/v1/geo/districts/{cantonId}` | Districts of a canton |
| GET | `/api/v1/geo/districts/{cantonId}/{districtId}` | District by ID |

#### Volcano API

Enriches location data in real time by querying the Geo API concurrently (`Task.WhenAll`).

| Method | Route | Description |
|---|---|---|
| GET | `/api/v1/volcanoes/` | List all volcanoes |
| GET | `/api/v1/volcanoes/{id}` | Volcano by ID |
| GET | `/api/v1/volcanoes/province/{provinceId}` | Volcanoes by province |

### Infrastructure Components

- **AppHost with YARP Gateway** — .NET Aspire orchestrator with integrated API Gateway for routing
- **ServiceDefaults** — Shared configuration (OpenTelemetry, health checks, Entra ID auth, Redis, EF, Refit)
- **Shared Library** — DTOs, interfaces, constants, cache keys, route constants, `Result<T>` pattern
- **Source Generators** — Roslyn generators for DI registration (`[TransientService]`, `[ScopedService]`, `[SingletonService]`, `[DecoratorService]`) and authorization policies (`[AuthorizationPolicy]`)
- **Tests** — Integration tests using Aspire Testing

## Technology Stack

- **.NET 10** — Latest .NET framework
- **.NET Aspire** — Cloud-native local orchestration with integrated YARP
- **ASP.NET Core Minimal APIs** — Lightweight API endpoints
- **Entity Framework Core** — ORM for SQL Server data access
- **Azure SQL Database** — Primary relational database (shared, schema-separated)
- **Redis** — Distributed cache (`Aspire.StackExchange.Redis`)
- **Azure Cosmos DB (MongoDB API)** — NoSQL database for application logging
- **Microsoft Entra ID (Azure AD)** — Authentication and authorization with JWT Bearer tokens
- **YARP (Yet Another Reverse Proxy)** — API Gateway for request routing
- **OpenTelemetry** — Distributed tracing, metrics, and structured logging
- **Refit** — Type-safe REST client for service-to-service calls
- **AutoMapper** — Object mapping
- **Asp.Versioning** — API versioning (`/api/v{version}/...`)
- **Scalar** — Interactive OpenAPI documentation at `/docs`
- **Azure Container Apps** — Production deployment target
- **Azure Developer CLI (`azd`)** — Deployment orchestration
- **Docker Compose** — Local environment without Aspire

## Authentication & Authorization

The application uses **Microsoft Entra ID (formerly Azure AD)** for secure authentication and authorization:

- **JWT Bearer Token Authentication** — All microservices validate JWT tokens issued by Entra ID
- **Scope-Based Authorization** — Fine-grained policies generated at compile time from `[AuthorizationPolicy]` attributes; enforced by checking the `roles` claim in the JWT token
- **Service-to-Service Auth** — The `DiscoverCostaRicaAuthHandler` delegating handler automatically attaches a Bearer token when calling other services

### Roles

| Role | Description |
|---|---|
| `Administrator` | Full access, including destructive operations |
| `Writer` | Can create and modify resources |
| `Reader` | Read-only access |
| `User` | Authenticated user with no special permissions |

### Environment Parameters (per service)

| Parameter | Description |
|---|---|
| `EntraId__Audience` | App ID URI |
| `EntraId__Instance` | `https://login.microsoftonline.com/` |
| `EntraId__ClientId` | Application (Client) ID |
| `EntraId__TenantId` | Directory (Tenant) ID |
| `Azure__TenantId` | Tenant for service-to-service token acquisition |
| `Azure__ClientId` | Client ID for service-to-service tokens |
| `Azure__ClientSecret` | Client Secret |
| `Azure__Scope` | Scope for service-to-service tokens |

## Caching

Redis is used as a distributed cache via the **Decorator pattern**: a cache-aware class wraps the base service without changing its interface. The `[DecoratorService]` attribute causes the Source Generator to register the decorator automatically.

| Cache Key | Cached Data |
|---|---|
| `Geo.Provinces` | Full list of provinces |
| `Beach.Beaches` | Full list of beaches |
| `Culture.Dishes` | Full list of dishes |
| `Culture.Traditions` | Full list of traditions |
| `Volcano.Volcanos` | Full list of volcanoes |

Cache invalidation is TTL-based (configured in Redis). Write operations do not explicitly invalidate the cache in the current version.

## Observability

### OpenTelemetry

All services are instrumented via `ServiceDefaults`:

| Signal | Instrumentation |
|---|---|
| **Traces** | ASP.NET Core + HTTP Client (excludes `/health` and `/alive`) |
| **Metrics** | ASP.NET Core + HTTP Client + Runtime |
| **Logs** | OpenTelemetry Logging with scopes and formatted messages |

The exporter is configured via the `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable (compatible with the Aspire Dashboard and external services).

### Health Checks

Each service exposes (development only):
- `GET /health` — all registered checks must pass
- `GET /alive` — liveness probe (checks tagged `live`)

### Custom Logging (MongoDB)

`DiscoverCostaRicaLoggerProvider` persists structured log entries to MongoDB, making application logs queryable from the database. Each entry includes `Timestamp`, `Level`, `Message`, `Exception`, and `ServiceName`.

## API Versioning

Routes follow the pattern `/api/v{version}/{resource}` (e.g., `/api/v1/beaches/`). The version is negotiated via URL path.

- **Current version:** v1.0
- **Defined:** v1.0, v2.0 (v2.0 not yet implemented)
- **Deprecation policy:** A version is supported for a minimum of 12 months after the next version is released in production.

## Project Structure

```
DiscoverCostaRica/
├── DiscoverCostaRica.sln
├── azure.yaml                          # Azure Developer CLI configuration
├── docker-compose.yml                  # Local environment without Aspire
├── Makefile                            # Development commands
│
├── DiscoverCostaRica.AppHost/          # .NET Aspire orchestrator + YARP Gateway
├── DiscoverCostaRica.ServiceDefaults/  # Shared configuration (OTel, Auth, Redis, EF)
├── DiscoverCostaRica.Shared/           # DTOs, interfaces, constants, attributes
├── DiscoverCostaRica.SourceGenerators/ # Roslyn generators (DI + Policies)
├── DiscoverCostaRica.Tests/            # Integration tests (Aspire Testing)
│
└── src/
    ├── DiscoverCostaRica.Beaches/
    │   ├── DiscoverCostaRica.Beaches.Domain/
    │   ├── DiscoverCostaRica.Beaches.Application/
    │   ├── DiscoverCostaRica.Beaches.Infrastructure/
    │   └── DiscoverCostaRica.Beaches.Api/
    │
    ├── DiscoverCostaRica.Culture/
    │   ├── DiscoverCostaRica.Culture.Domain/
    │   ├── DiscoverCostaRica.Culture.Application/
    │   ├── DiscoverCostaRica.Culture.Infrastructure/
    │   └── DiscoverCostaRica.Culture.Api/
    │
    ├── DiscoverCostaRica.Geo/
    │   ├── DiscoverCostaRica.Geo.Domain/
    │   ├── DiscoverCostaRica.Geo.Application/
    │   ├── DiscoverCostaRica.Geo.Infrastructure/
    │   └── DiscoverCostaRica.Geo.Api/
    │
    └── DiscoverCostaRica.Volcano/
        ├── DiscoverCostaRica.Volcano.Domain/
        ├── DiscoverCostaRica.Volcano.Application/
        ├── DiscoverCostaRica.Volcano.Infrastructure/
        └── DiscoverCostaRica.Volcano.Api/
```

## Getting Started

### With .NET Aspire (recommended)

```bash
dotnet run --project DiscoverCostaRica.AppHost
# or
make run
```

The AppHost starts all services with their dependencies injected automatically. The Aspire Dashboard is available at `http://localhost:18888`.

### With Docker Compose

```bash
docker compose up
```

Starts the 4 microservices + SQL Server 2022 + Redis 7 + MongoDB 7, without requiring .NET Aspire.

| Service | Port |
|---|---|
| beaches-api | 7000 |
| culture-api | 7001 |
| volcano-api | 7002 |
| geo-api | 7003 |
| SQL Server | 1433 |
| Redis | 6379 |
| MongoDB | 27017 |

Each service exposes interactive API documentation at `/docs` (Scalar/OpenAPI).

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
