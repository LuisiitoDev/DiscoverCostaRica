# System Architecture Document — Discover Costa Rica

**Version:** 1.0  
**Date:** April 2026  
**Status:** Current

---

## Table of Contents

1. [Overview](#1-overview)
2. [System Context](#2-system-context)
3. [Architecture Decisions](#3-architecture-decisions)
4. [Solution Structure](#4-solution-structure)
5. [Microservices](#5-microservices)
6. [Internal Architecture — Clean Architecture](#6-internal-architecture--clean-architecture)
7. [Data Model](#7-data-model)
8. [Service-to-Service Communication](#8-service-to-service-communication)
9. [Security and Authentication](#9-security-and-authentication)
10. [Cache and Performance](#10-cache-and-performance)
11. [Observability](#11-observability)
12. [Source Code Generators](#12-source-code-generators)
13. [Infrastructure and Deployment](#13-infrastructure-and-deployment)
14. [API Versioning](#14-api-versioning)
15. [Response Patterns](#15-response-patterns)
16. [Component Diagram](#16-component-diagram)

---

## 1. Overview

**Discover Costa Rica** is a tourism information platform exposed as a set of RESTful microservices. It provides data about:

- **Beaches** — listing and detail of the country's beaches.
- **Volcanoes** — geolocated volcano information, enriched with geographic data.
- **Culture** — typical dishes and cultural traditions.
- **Geography** — administrative hierarchy Province → Canton → District.

The solution is built on **.NET 10** with **.NET Aspire** as the local development orchestrator and **Azure Container Apps** as the production target.

---

## 2. System Context

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

### External Actors

| Actor | Description |
|---|---|
| HTTP Clients | API consumers (apps, browsers, external services) |
| Microsoft Entra ID | OAuth2 / OIDC identity provider for authentication and authorization |
| Azure Developer CLI (`azd`) | Deployment tool to Azure Container Apps |

---

## 3. Architecture Decisions

### ADR-01 — Microservices with Clean Architecture

Each business domain (Beaches, Volcanoes, Culture, Geography) lives in its own independent microservice. Within each service, **Clean Architecture** is applied with four layers: Domain → Application → Infrastructure → API.

**Rationale:** Independent scalability, autonomous deployment, well-defined domain boundaries.

### ADR-02 — Shared Database with Schema Separation

All services point to the same Azure SQL database, but each one operates on its own schema (`Beach.Beach`, `Volcano.Volcano`, etc.).

**Rationale:** At the current scale, a database-per-service adds unnecessary operational complexity. Geographic tables are read-only for other services, so coupling is minimal.

### ADR-03 — .NET Aspire for Local Orchestration

The `DiscoverCostaRica.AppHost` project acts as the development orchestrator, injecting connection strings, configuration parameters, and managing startup dependencies.

**Rationale:** Simplifies bootstrapping the full environment locally without managing multiple processes manually.

### ADR-04 — Source Generators for DI Registration

Instead of registering services manually, Roslyn Source Generators are used that inspect attributes (`[TransientService]`, `[ScopedService]`, `[SingletonService]`, `[DecoratorService]`) and automatically generate the registration code.

**Rationale:** Eliminates repetitive manual registration and prevents omission errors. Generated code is verifiable at compile time.

### ADR-05 — Microsoft Entra ID as Identity Provider

All authentication is delegated to Microsoft Entra ID (Azure AD). Services validate JWT Bearer tokens. Authorization policies are also generated via Source Generators from the `[AuthorizationPolicy]` attribute.

**Rationale:** Enterprise identity management without implementing custom authentication logic.

### ADR-06 — YARP as API Gateway

**YARP (Yet Another Reverse Proxy)** integrated in Aspire is used to route external requests to microservices, applying route prefix transformations.

**Rationale:** Single entry point, hides the internal service topology from clients.

### ADR-07 — Redis for Distributed Cache with Decorator Pattern

Caching is implemented with the **Decorator pattern**: a cache-aware service class wraps the base service without modifying its interface.

**Rationale:** Transparency for consumers, easy to enable/disable, allows selective invalidation.

---

## 4. Solution Structure

```
DiscoverCostaRica/
├── DiscoverCostaRica.sln
├── azure.yaml                          ← Azure Developer CLI configuration
├── docker-compose.yml                  ← Local environment without Aspire
├── Makefile                            ← Development commands
│
├── DiscoverCostaRica.AppHost/          ← .NET Aspire orchestrator
├── DiscoverCostaRica.ServiceDefaults/  ← Shared configuration (OTel, Auth, Redis, EF)
├── DiscoverCostaRica.Shared/           ← DTOs, interfaces, constants, attributes
├── DiscoverCostaRica.SourceGenerators/ ← Roslyn generators (DI + Policies)
├── DiscoverCostaRica.Tests/            ← Integration tests (Aspire Testing)
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

---

## 5. Microservices

### 5.1 Beaches API

| Attribute | Value |
|---|---|
| Aspire Name | `beachesservice` |
| Base URL (Gateway) | `/beaches/` |
| Base URL (Internal) | `/api/v1/beaches/` |
| Database | Schema `Beach` |
| Read Access Policy | `Beaches.Read` |

**Domain:** Manages information about Costa Rican beaches. Simple entity with `Id`, `Name`, `Description`.

**Exposed endpoints:**

| Method | Route | Description | Authorization |
|---|---|---|---|
| GET | `/api/v1/beaches/` | List all beaches | `Beaches.Read` |

### 5.2 Culture API

| Attribute | Value |
|---|---|
| Aspire Name | `cultureservice` |
| Base URL (Gateway) | `/tradition/`, `/dish/` |
| Base URL (Internal) | `/api/v1/traditions/` |
| Database | Schema `Culture` |
| Read Access Policy | `Culture.Read` |

**Domain:** Manages typical dishes (`DishModel`) and cultural traditions (`TraditionModel`).

**Exposed endpoints:**

| Method | Route | Description | Authorization |
|---|---|---|---|
| GET | `/api/v1/traditions/dish` | List typical dishes | `Culture.Read` |
| GET | `/api/v1/traditions/tradition` | List traditions | `Culture.Read` |

### 5.3 Geo API

| Attribute | Value |
|---|---|
| Aspire Name | `geoservice` |
| Base URL (Gateway) | `/provinces/`, `/canton/`, `/districts/` |
| Base URL (Internal) | `/api/v1/geo/` |
| Database | Schema `Geo` |
| Read Access Policy | `Geo.Read` |
| Dependencies | SQL Server |

**Domain:** Administrative hierarchy of Costa Rica: 7 Provinces → 82 Cantons → ~488 Districts. Uses composite keys for Canton (`Id`, `ProvinceId`) and District (`Id`, `CantonId`, `CantonProvinceId`).

**Exposed endpoints:**

| Method | Route | Description | Authorization |
|---|---|---|---|
| GET | `/api/v1/geo/provinces` | List provinces | `Geo.Read` |
| GET | `/api/v1/geo/provinces/{provinceId}` | Province by ID | `Geo.Read` |
| GET | `/api/v1/geo/cantons/{provinceId}` | Cantons of a province | `Geo.Read` |
| GET | `/api/v1/geo/cantons/{provinceId}/{cantonId}` | Canton by ID | `Geo.Read` |
| GET | `/api/v1/geo/districts/{cantonId}` | Districts of a canton | `Geo.Read` |
| GET | `/api/v1/geo/districts/{cantonId}/{districtId}` | District by ID | `Geo.Read` |

> **Note:** The Geo Service acts as a support service for the Volcano API, which queries it to enrich location data.

### 5.4 Volcano API

| Attribute | Value |
|---|---|
| Aspire Name | `volcanoservice` |
| Base URL (Gateway) | `/volcano/`, `/province/` |
| Base URL (Internal) | `/api/v1/volcanoes/` |
| Database | Schema `Volcano` |
| Read Access Policy | `Volcano.Read` |
| Dependencies | SQL Server, Redis, Geo API |

**Domain:** Manages volcanoes with a reference to the geographic hierarchy (`ProvinceId`, `CantonId`, `DistrictId?`). Location data is enriched in real time from the Geo Service via concurrent HTTP calls (`Task.WhenAll`).

**Exposed endpoints:**

| Method | Route | Description | Authorization |
|---|---|---|---|
| GET | `/api/v1/volcanoes/` | List all volcanoes | `Volcano.Read` |
| GET | `/api/v1/volcanoes/{id}` | Volcano by ID | `Volcano.Read` |
| GET | `/api/v1/volcanoes/province/{provinceId}` | Volcanoes by province | `Volcano.Read` |

---

## 6. Internal Architecture — Clean Architecture

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

### HTTP Request Flow

```
HTTP Request
     │
     ▼
[API Layer] Handler receives the request, performs shallow validation
     │  invokes
     ▼
[Application Layer] IService → business logic, DTO mapping
     │  if cache available → CacheService (Decorator)
     │  if not → Repository
     ▼
[Domain Layer] Pure entities, business rules
     │
     ▼
[Infrastructure Layer] EF Core → Azure SQL Server
     │
     ▼
HTTP Response (Result<T> → IResult via ToResult())
```

### Decorator Pattern for Cache

```csharp
// Source Generator registers:
services.AddTransient<IBeachService, BeachService>();
services.Decorate<IBeachService, CacheBeachService>();

// CacheBeachService wraps BeachService:
public class CacheBeachService(IBeachService inner, ICacheService cache) : IBeachService
{
    public async Task<Result<List<DtoBeach>>> GetBeaches(CancellationToken ct)
    {
        var cached = await cache.Get<Result<List<DtoBeach>>>(CacheKeys.Beach.BEACHES, ct);
        if (cached is not null) return cached;
        var result = await inner.GetBeaches(ct);
        if (result.StatusCode == 200) await cache.Set(CacheKeys.Beach.BEACHES, result, ct);
        return result;
    }
}
```

---

## 7. Data Model

### 7.1 Beaches

```
Beach.Beach
├── Id          INT IDENTITY PK
├── Name        NVARCHAR(1000) NOT NULL
└── Description NVARCHAR(10000) NOT NULL
```

### 7.2 Culture

```
Culture.Dish
├── Id           INT IDENTITY PK
├── Name         NVARCHAR NOT NULL
├── Description  NVARCHAR NOT NULL
├── Ingredients  NVARCHAR NOT NULL
├── Preparation  NVARCHAR NOT NULL
└── ImageUrl     NVARCHAR

Culture.Tradition
├── Id          INT IDENTITY PK
├── Name        NVARCHAR NOT NULL
├── Description NVARCHAR NOT NULL
└── ImageUrl    NVARCHAR
```

### 7.3 Geo (Administrative Hierarchy)

```
Geo.Province
├── Id    INT PK
└── Name  NVARCHAR NOT NULL
   │
   └──< Geo.Canton
        ├── Id          INT      ┐ Composite PK
        ├── ProvinceId  INT      ┘ FK → Province
        ├── Name        NVARCHAR NOT NULL
        │
        └──< Geo.District
             ├── Id              INT  ┐
             ├── CantonId        INT  │ Composite PK
             ├── CantonProvinceId INT ┘ FK → Canton
             └── Name            NVARCHAR NOT NULL
```

### 7.4 Volcano

```
Volcano.Volcano
├── Id          INT IDENTITY PK
├── ProvinceId  INT NOT NULL   (logical reference to Geo.Province)
├── CantonId    INT NOT NULL   (logical reference to Geo.Canton)
├── DistrictId  INT NULL       (logical reference to Geo.District)
├── Name        NVARCHAR NOT NULL
└── Description NVARCHAR NOT NULL
```

> References between Volcano and Geo are **logical** (no physical FKs across schemas). Resolution is done at query time via calls to the Geo API.

---

## 8. Service-to-Service Communication

### 8.1 Volcano → Geo (via Refit + Service Discovery)

The Volcano Service enriches location data by querying the Geo Service using a typed Refit client (`IGeoDiscoverCostaRica`), registered automatically in `ServiceDefaults`.

```
Volcano API
    │
    ├── For each volcano: GetProvinceById + GetCantonById + GetDistrictById
    │   (executed CONCURRENTLY with Task.WhenAll)
    │
    └── Geo API (/api/v1/geo/...)
```

The service discovery (Aspire Service Discovery) resolves `https://geoservice` to the real container address at runtime.

### 8.2 Service-to-Service Authentication

Calls between services use `DiscoverCostaRicaAuthHandler` (a `DelegatingHandler`) that automatically attaches a Bearer token from Entra ID using `DiscoverCostaRicaTokenAcquisitionService`.

```csharp
// Configuration in ServiceDefaults
services.AddRefitClient<IGeoDiscoverCostaRica>()
    .ConfigureHttpClient(http => http.BaseAddress = new Uri("https://geoservice/api/v1/geo"))
    .AddHttpMessageHandler<DiscoverCostaRicaAuthHandler>()
    .AddStandardResilienceHandler();  // ← retries, circuit breaker, timeouts
```

### 8.3 Resilience

All HTTP clients use `AddStandardResilienceHandler()` from .NET Resilience, which automatically configures:
- **Retries** with exponential backoff
- **Circuit Breaker** to isolate failures
- **Timeout** per request and total

---

## 9. Security and Authentication

### 9.1 Authentication

All services validate **JWT Bearer tokens** issued by **Microsoft Entra ID**. Configuration is centralized in `ServiceDefaults.AddEntraIdAuthentication()`.

```
Client → [Bearer Token] → Service → [Validates with Entra ID JWKS]
```

Configurable parameters per environment (injected by Aspire):
- `EntraId__Audience` — App ID URI of the registered client
- `EntraId__Instance` — Entra ID instance endpoint
- `EntraId__ClientId` — Application Client ID
- `EntraId__TenantId` — Azure Tenant ID

### 9.2 Authorization — Scope-Based Policies

Policies are automatically generated via Source Generators from `DiscoverPolicies.cs`:

```csharp
[AuthorizationPolicy("Beaches.Read",  "Beaches.Read")]
[AuthorizationPolicy("Volcano.Read",  "Volcano.Read")]
[AuthorizationPolicy("Culture.Read",  "Culture.Read")]
[AuthorizationPolicy("Geo.Read",      "Geo.Read")]
public class DiscoverPolicies { }
```

The generator produces `AddPolicies()` which checks the `roles` claim in the JWT token. Policies are applied to endpoints with `.RequireAuthorization("Beaches.Read")`.

### 9.3 Roles

| Role | Description |
|---|---|
| `Administrator` | Full access, including destructive operations |
| `Writer` | Can create and modify resources |
| `Reader` | Read-only access |
| `User` | Authenticated user with no special permissions |

### 9.4 Custom Claims

`ICurrentUserService` exposes the authenticated user context (implemented over `IHttpContextAccessor`), allowing access to the current user from any application layer.

---

## 10. Cache and Performance

### 10.1 Redis as Distributed Cache

**Redis** (integrated via `Aspire.StackExchange.Redis`) is used as a distributed cache. The `ICacheService` interface abstracts the cache operations:

```csharp
public interface ICacheService
{
    Task<T?> Get<T>(string key, CancellationToken cancellationToken);
    Task Set<T>(string key, T value, CancellationToken cancellationToken);
}
```

### 10.2 Cache Keys

| Key | Cached Data |
|---|---|
| `Geo.Provinces` | Full list of provinces |
| `Beach.Beaches` | Full list of beaches |
| `Culture.Dishes` | Full list of dishes |
| `Culture.Traditions` | Full list of traditions |
| `Volcano.Volcanos` | Full list of volcanoes |

### 10.3 Invalidation Strategy

Currently the cache is invalidated by TTL (expiration time defined in the Redis configuration). Write operations do not explicitly invalidate the cache in the current version.

---

## 11. Observability

### 11.1 OpenTelemetry

All services have OpenTelemetry instrumentation configured in `ServiceDefaults`:

| Signal | Instrumentation |
|---|---|
| **Traces** | ASP.NET Core + HTTP Client (excludes `/health` and `/alive`) |
| **Metrics** | ASP.NET Core + HTTP Client + Runtime |
| **Logs** | OpenTelemetry Logging with scopes and formatted message |

The exporter is configured via the `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable (compatible with Aspire Dashboard and external services).

### 11.2 Aspire Dashboard

In the development environment, the Aspire Dashboard provides:
- Distributed trace visualization
- Real-time metrics
- Structured logs correlated by TraceId

### 11.3 Health Checks

Each service exposes (Development only):
- `GET /health` — all checks must pass for the service to be considered ready
- `GET /alive` — only checks with the `live` tag, for Kubernetes/Container Apps liveness probe

### 11.4 Custom Logging (MongoDB)

`DiscoverCostaRicaLoggerProvider` + `DiscoverCostaRicaLogger` provide a logger that persists entries in **MongoDB**, allowing application logs to be queried from the database.

Log entry structure (`LogEntryModel`):
```json
{
  "Timestamp": "2026-04-10T18:00:00Z",
  "Level": "Error",
  "Message": "...",
  "Exception": "...",
  "ServiceName": "beachesservice"
}
```

---

## 12. Source Code Generators

The solution uses two Roslyn IIncrementalGenerators registered as analyzers:

### 12.1 ServiceRegistrationGenerator

Scans the assembly for classes with lifetime attributes and generates:

```csharp
// Automatically generated at compile time:
public static class ServiceRegistrationExtensions_DiscoverCostaRica_Beaches_Application
{
    public static IServiceCollection AddGeneratedServices_DiscoverCostaRica_Beaches_Application(
        this IServiceCollection services)
    {
        services.AddTransient<IBeachRepository, BeachRepository>();
        services.AddTransient<IBeachService, BeachService>();
        services.Decorate<IBeachService, CacheBeachService>();
        // ...
        return services;
    }
}
```

| Attribute | Lifetime | Behavior |
|---|---|---|
| `[TransientService]` | Transient | Registers `services.AddTransient<IFoo, Foo>()` |
| `[ScopedService]` | Scoped | Registers `services.AddScoped<IFoo, Foo>()` |
| `[SingletonService]` | Singleton | Registers `services.AddSingleton<IFoo, Foo>()` |
| `[DecoratorService]` | — | Registers with Scrutor `services.Decorate<IFoo, DecoratorFoo>()` |

### 12.2 AuthorizationPolicyGenerator

Scans classes decorated with `[AuthorizationPolicy(policyName, scope)]` and generates the `AddPolicies()` method to register scope-based authorization policies:

```csharp
// Automatically generated:
public static IServiceCollection AddPolicies(this IServiceCollection services)
{
    services.AddAuthorizationBuilder()
        .AddPolicy("Beaches.Read", policy =>
            policy.RequireAssertion(ctx =>
                ctx.User.Claims
                    .Where(c => c.Type == AuthConstants.ClaimTypes.Roles)
                    .Any(c => c.Value.Split(' ').Contains("Beaches.Read"))))
        // ...
    return services;
}
```

---

## 13. Infrastructure and Deployment

### 13.1 Azure (Production)

| Resource | Azure Service |
|---|---|
| Microservices (×4) | Azure Container Apps |
| Database | Azure SQL Server (existing, referenced as parameter) |
| Cache | Azure Cache for Redis (external connection string) |
| Logs | Azure Cosmos DB with MongoDB API (external connection string) |
| Identity | Microsoft Entra ID |
| Deploy Orchestration | Azure Developer CLI (`azd`) |

The `azure.yaml` file declares the 4 services as Container Apps. Sensitive parameters (Entra ID credentials) are injected as Aspire secret parameters at deployment time.

### 13.2 Environment Parameters

| Parameter | Description |
|---|---|
| `EntraId__Audience` | App ID URI |
| `EntraId__Instance` | `https://login.microsoftonline.com/` |
| `EntraId__ClientId` | Application (Client) ID |
| `EntraId__TenantId` | Directory (Tenant) ID |
| `Azure__TenantId` | Tenant for service-to-service token acquisition |
| `Azure__ClientId` | Client ID for service-to-service tokens |
| `Azure__ClientSecret` | Client Secret (secret) |
| `Azure__Scope` | Scope for service-to-service tokens |
| `existingSqlServerName` | Name of the existing Azure SQL Server |
| `existingSqlServerResourceGroup` | Resource Group of the SQL Server |

### 13.3 Local Development with .NET Aspire

```bash
dotnet run --project DiscoverCostaRica.AppHost
# or
make run
```

The AppHost starts all services with their dependencies injected automatically. The Aspire Dashboard is available at `http://localhost:18888`.

### 13.4 Local Development with Docker Compose

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

---

## 14. API Versioning

Routes follow the pattern `/api/v{version}/{resource}` (example: `/api/v1/beaches/`).

The version is negotiated via URL path. Configuration in `ServiceDefaults`:

```csharp
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;  // ← returns api-supported-versions header
});
```

**Declared versions:** v1.0, v2.0 (v2.0 defined but not yet implemented).

**Deprecation policy:** A version is supported for a minimum of 12 months after the next version is released in production.

---

## 15. Response Patterns

### 15.1 Result Pattern

All business operations return `Result<T>`, a discriminated union of three variants:

```csharp
// Generic response
record Result(int StatusCode, string? Message);

// Success
sealed record Success(object Value, int StatusCode = 200) : Result;

// Error
sealed record Failure(string Message, int StatusCode = 500) : Result;

// Typed Result — combines the above via implicit conversion
sealed record Result<TResult>(int StatusCode, string? Message) : Result
{
    TResult? Value { get; set; }
    static implicit operator Result<TResult>(Success s) => ...;
    static implicit operator Result<TResult>(Failure f) => ...;
}
```

### 15.2 Conversion to IResult (HTTP)

The `ToResult()` extension method converts `Result<T>` to the Minimal APIs `IResult`:

| StatusCode | HTTP Response |
|---|---|
| 200 | `Results.Ok(result)` |
| 404 | `Results.NotFound(result)` |
| 400 | `Results.BadRequest(result)` |
| 500 | `Results.InternalServerError()` |
| Other | `Results.Problem(message, statusCode)` |

### 15.3 Global Exception Handling

`GlobalExceptionHandler` (registered in all services) catches unhandled exceptions and returns a `ProblemDetails` with status 500, without exposing internal details to the client.

### 15.4 OpenAPI Documentation

Each service exposes its OpenAPI specification at `/openapi/v1.json` and an interactive **Scalar** interface at `/docs`.

---

## 16. Component Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         DiscoverCostaRica.Shared                        │
│  AuthConstants · Roles · Scopes · Policies · CacheKeys · RoutesConstants│
│  Result<T> · Success · Failure · ICacheService · IGeoDiscoverCostaRica   │
│  [TransientService] · [DecoratorService] · [AuthorizationPolicy]        │
└─────────────────────────────────────────────────────────────────────────┘
         ▲ referenced by all projects

┌─────────────────────────────────────────────────────────────────────────┐
│                      DiscoverCostaRica.ServiceDefaults                  │
│  AddServiceDefaults() · AddEntraIdAuthentication() · AddVersioning()    │
│  ConfigureOpenTelemetry() · GlobalExceptionHandler                      │
│  DiscoverCostaRicaAuthHandler (service-to-service)                      │
└─────────────────────────────────────────────────────────────────────────┘
         ▲ referenced by all API services

┌─────────────────────────────────────────────────────────────────────────┐
│                   DiscoverCostaRica.SourceGenerators                    │
│  ServiceRegistrationGenerator  →  generates AddGeneratedServices_*()   │
│  AuthorizationPolicyGenerator  →  generates AddPolicies()               │
└─────────────────────────────────────────────────────────────────────────┘
         ▲ Analyzer in Application and Infrastructure layers

┌──────────────────────────┐  ┌──────────────────────────┐
│   Beaches Service        │  │   Culture Service         │
│  ┌────────────────────┐  │  │  ┌────────────────────┐  │
│  │ Domain             │  │  │  │ Domain (Dish,       │  │
│  │ (BeachModel)       │  │  │  │  Tradition)         │  │
│  ├────────────────────┤  │  │  ├────────────────────┤  │
│  │ Application        │  │  │  │ Application        │  │
│  │ (BeachService,     │  │  │  │ (CultureService,   │  │
│  │  CacheBeachService)│  │  │  │  CachedCulture..)  │  │
│  ├────────────────────┤  │  │  ├────────────────────┤  │
│  │ Infrastructure     │  │  │  │ Infrastructure     │  │
│  │ (BeachRepository,  │  │  │  │ (CultureRepository)│  │
│  │  BeachContext/EF)  │  │  │  │                    │  │
│  ├────────────────────┤  │  │  ├────────────────────┤  │
│  │ API (Minimal API)  │  │  │  │ API (Minimal API)  │  │
│  └────────────────────┘  │  │  └────────────────────┘  │
└──────────────────────────┘  └──────────────────────────┘

┌──────────────────────────┐  ┌──────────────────────────┐
│   Geo Service            │  │   Volcano Service         │
│  ┌────────────────────┐  │  │  ┌────────────────────┐  │
│  │ Domain             │  │  │  │ Domain             │  │
│  │ (Province, Canton, │  │  │  │ (VolcanoModel)     │  │
│  │  District)         │  │  │  ├────────────────────┤  │
│  ├────────────────────┤  │  │  │ Application        │  │
│  │ Application        │  │  │  │ (VolcanoService,   │  │
│  │ (ProvinceService,  │  │  │  │  LocationService → │  │
│  │  GeoService,       │  │  │  │  calls Geo API)    │  │
│  │  CachedProvince..) │  │  │  ├────────────────────┤  │
│  ├────────────────────┤  │  │  │ Infrastructure     │  │
│  │ Infrastructure     │  │  │  │ (VolcanoRepository,│  │
│  │ (GeoRepository,    │  │  │  │  GeoDataProvider)  │  │
│  │  GeoContext/EF)    │  │  │  ├────────────────────┤  │
│  ├────────────────────┤  │  │  │ API (Minimal API)  │  │
│  │ API (Minimal API)  │  │  │  └────────────────────┘  │
│  └────────────────────┘  │  └──────────────────────────┘
└──────────────────────────┘

                    │                    │
                    ▼                    ▼
         ┌──────────────────────────────────────┐
         │           Azure SQL Server            │
         │  Beach.* | Culture.* | Geo.* | Vol.*  │
         └──────────────────────────────────────┘
                    │
         ┌──────────▼──────────┐
         │    Redis Cache      │
         │  (frequent data)    │
         └─────────────────────┘
```

---

## References

| Resource | Link / Location |
|---|---|
| .NET Aspire | `https://learn.microsoft.com/dotnet/aspire` |
| YARP Reverse Proxy | `https://microsoft.github.io/reverse-proxy/` |
| Refit HTTP Client | `https://github.com/reactiveui/refit` |
| Scrutor (Decorator DI) | `https://github.com/khellang/Scrutor` |
| Asp.Versioning | `https://github.com/dotnet/aspnet-api-versioning` |
| Scalar OpenAPI UI | `https://scalar.com` |
| Roslyn Source Generators | `DiscoverCostaRica.SourceGenerators/` |
| Aspire Configuration | `DiscoverCostaRica.AppHost/AppHost.cs` |
| Shared Defaults | `DiscoverCostaRica.ServiceDefaults/Extensions.cs` |
| API Routes | `DiscoverCostaRica.Shared/Routes/RoutesConstants.cs` |
| Authorization Policies | `DiscoverCostaRica.Shared/Authentication/DiscoverPolicies.cs` |
| Azure Deployment | `azure.yaml` |
