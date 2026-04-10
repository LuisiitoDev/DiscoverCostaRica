# Documento de Arquitectura del Sistema — Discover Costa Rica

**Versión:** 1.0  
**Fecha:** Abril 2026  
**Estado:** Vigente

---

## Tabla de Contenidos

1. [Visión General](#1-visión-general)
2. [Contexto del Sistema](#2-contexto-del-sistema)
3. [Decisiones de Arquitectura](#3-decisiones-de-arquitectura)
4. [Estructura de la Solución](#4-estructura-de-la-solución)
5. [Microservicios](#5-microservicios)
6. [Arquitectura Interna — Clean Architecture](#6-arquitectura-interna--clean-architecture)
7. [Modelo de Datos](#7-modelo-de-datos)
8. [Comunicación entre Servicios](#8-comunicación-entre-servicios)
9. [Seguridad y Autenticación](#9-seguridad-y-autenticación)
10. [Caché y Rendimiento](#10-caché-y-rendimiento)
11. [Observabilidad](#11-observabilidad)
12. [Generadores de Código Fuente](#12-generadores-de-código-fuente)
13. [Infraestructura y Despliegue](#13-infraestructura-y-despliegue)
14. [Versionamiento de API](#14-versionamiento-de-api)
15. [Patrones de Respuesta](#15-patrones-de-respuesta)
16. [Diagrama de Componentes](#16-diagrama-de-componentes)

---

## 1. Visión General

**Discover Costa Rica** es una plataforma de información turística expuesta como un conjunto de microservicios RESTful. Provee datos sobre:

- **Playas** — listado y detalle de playas del país.
- **Volcanes** — información geolocalizada de volcanes, enriquecida con datos geográficos.
- **Cultura** — platos típicos y tradiciones culturales.
- **Geografía** — jerarquía administrativa Provincia → Cantón → Distrito.

La solución está construida sobre **.NET 10** con **.NET Aspire** como orquestador de desarrollo local y **Azure Container Apps** como destino de producción.

---

## 2. Contexto del Sistema

```
┌─────────────────────────────────────────────────────────┐
│                      CLIENTES                           │
│   (Navegadores, Apps Móviles, Otros Servicios)          │
└────────────────────────┬────────────────────────────────┘
                         │ HTTPS
                         ▼
┌────────────────────────────────────────────────────────┐
│              YARP API GATEWAY (gateway)                │
│   Enruta y transforma rutas públicas hacia servicios   │
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
     │  (compartida)   │    │  (caché distribuida) │
     └─────────────────┘    └──────────────────────┘
              │
     ┌────────▼────────┐
     │  MongoDB        │
     │  (logs)         │
     └─────────────────┘
```

### Actores externos

| Actor | Descripción |
|---|---|
| Clientes HTTP | Consumidores de la API (apps, navegadores, servicios externos) |
| Microsoft Entra ID | Proveedor de identidad OAuth2 / OIDC para autenticación y autorización |
| Azure Developer CLI (`azd`) | Herramienta de despliegue a Azure Container Apps |

---

## 3. Decisiones de Arquitectura

### ADR-01 — Microservicios con Clean Architecture

Cada dominio de negocio (Playas, Volcanes, Cultura, Geografía) vive en su propio microservicio independiente. Dentro de cada servicio se aplica **Clean Architecture** con cuatro capas: Domain → Application → Infrastructure → API.

**Motivo:** Escalabilidad independiente, despliegue autónomo, fronteras de dominio bien definidas.

### ADR-02 — Base de Datos Compartida con Separación por Esquema

Todos los servicios apuntan a la misma base de datos Azure SQL, pero cada uno opera sobre su propio esquema (`Beach.Beach`, `Volcano.Volcano`, etc.).

**Motivo:** Para la escala actual, una BD por servicio agrega complejidad operacional innecesaria. Las tablas geográficas son de solo lectura para los demás servicios, por lo que el acoplamiento es mínimo.

### ADR-03 — .NET Aspire para Orquestación Local

El proyecto `DiscoverCostaRica.AppHost` actúa como orquestador de desarrollo, inyectando cadenas de conexión, parámetros de configuración y administrando dependencias de arranque.

**Motivo:** Simplifica el arranque del entorno completo en local sin gestionar múltiples procesos manualmente.

### ADR-04 — Source Generators para Registro de DI

En lugar de registrar servicios manualmente, se usan Roslyn Source Generators que inspeccionan atributos (`[TransientService]`, `[ScopedService]`, `[SingletonService]`, `[DecoratorService]`) y generan el código de registro automáticamente.

**Motivo:** Elimina el registro manual repetitivo y previene errores de omisión. El código generado es verificable en compilación.

### ADR-05 — Microsoft Entra ID como Proveedor de Identidad

Toda autenticación se delega a Microsoft Entra ID (Azure AD). Los servicios validan JWT Bearer tokens. Las políticas de autorización se generan también vía Source Generators a partir del atributo `[AuthorizationPolicy]`.

**Motivo:** Gestión de identidad empresarial sin implementar lógica de autenticación propia.

### ADR-06 — YARP como API Gateway

Se usa **YARP (Yet Another Reverse Proxy)** integrado en Aspire para enrutar peticiones externas a los microservicios, aplicando transformaciones de prefijo de ruta.

**Motivo:** Punto único de entrada, oculta la topología interna de servicios a los clientes.

### ADR-07 — Redis para Caché Distribuida con Patrón Decorator

El caché se implementa con el **patrón Decorator**: una clase de servicio con caché envuelve al servicio base sin modificar su interfaz.

**Motivo:** Transparencia para los consumidores, fácil de activar/desactivar, permite invalidación selectiva.

---

## 4. Estructura de la Solución

```
DiscoverCostaRica/
├── DiscoverCostaRica.sln
├── azure.yaml                          ← Configuración Azure Developer CLI
├── docker-compose.yml                  ← Entorno local sin Aspire
├── Makefile                            ← Comandos de desarrollo
│
├── DiscoverCostaRica.AppHost/          ← Orquestador .NET Aspire
├── DiscoverCostaRica.ServiceDefaults/  ← Configuración compartida (OTel, Auth, Redis, EF)
├── DiscoverCostaRica.Shared/           ← DTOs, interfaces, constantes, atributos
├── DiscoverCostaRica.SourceGenerators/ ← Roslyn generators (DI + Policies)
├── DiscoverCostaRica.Tests/            ← Tests de integración (Aspire Testing)
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

## 5. Microservicios

### 5.1 Beaches API

| Atributo | Valor |
|---|---|
| Nombre en Aspire | `beachesservice` |
| Base URL (Gateway) | `/beaches/` |
| Base URL (Interno) | `/api/v1/beaches/` |
| Base de Datos | Esquema `Beach` |
| Política de Acceso Lectura | `Beaches.Read` |

**Dominio:** Gestiona información de playas costarricenses. Entidad simple con `Id`, `Name`, `Description`.

**Endpoints expuestos:**

| Método | Ruta | Descripción | Autorización |
|---|---|---|---|
| GET | `/api/v1/beaches/` | Lista todas las playas | `Beaches.Read` |

### 5.2 Culture API

| Atributo | Valor |
|---|---|
| Nombre en Aspire | `cultureservice` |
| Base URL (Gateway) | `/tradition/`, `/dish/` |
| Base URL (Interno) | `/api/v1/traditions/` |
| Base de Datos | Esquema `Culture` |
| Política de Acceso Lectura | `Culture.Read` |

**Dominio:** Gestiona platos típicos (`DishModel`) y tradiciones culturales (`TraditionModel`).

**Endpoints expuestos:**

| Método | Ruta | Descripción | Autorización |
|---|---|---|---|
| GET | `/api/v1/traditions/dish` | Lista platos típicos | `Culture.Read` |
| GET | `/api/v1/traditions/tradition` | Lista tradiciones | `Culture.Read` |

### 5.3 Geo API

| Atributo | Valor |
|---|---|
| Nombre en Aspire | `geoservice` |
| Base URL (Gateway) | `/provinces/`, `/canton/`, `/districts/` |
| Base URL (Interno) | `/api/v1/geo/` |
| Base de Datos | Esquema `Geo` |
| Política de Acceso Lectura | `Geo.Read` |
| Dependencias | SQL Server |

**Dominio:** Jerarquía administrativa de Costa Rica: 7 Provincias → 82 Cantones → ~488 Distritos. Usa claves compuestas para Cantón (`Id`, `ProvinceId`) y Distrito (`Id`, `CantonId`, `CantonProvinceId`).

**Endpoints expuestos:**

| Método | Ruta | Descripción | Autorización |
|---|---|---|---|
| GET | `/api/v1/geo/provinces` | Lista provincias | `Geo.Read` |
| GET | `/api/v1/geo/provinces/{provinceId}` | Provincia por ID | `Geo.Read` |
| GET | `/api/v1/geo/cantons/{provinceId}` | Cantones de una provincia | `Geo.Read` |
| GET | `/api/v1/geo/cantons/{provinceId}/{cantonId}` | Cantón por ID | `Geo.Read` |
| GET | `/api/v1/geo/districts/{cantonId}` | Distritos de un cantón | `Geo.Read` |
| GET | `/api/v1/geo/districts/{cantonId}/{districtId}` | Distrito por ID | `Geo.Read` |

> **Nota:** El Geo Service actúa como servicio de soporte para Volcano API, que lo consulta para enriquecer datos de ubicación.

### 5.4 Volcano API

| Atributo | Valor |
|---|---|
| Nombre en Aspire | `volcanoservice` |
| Base URL (Gateway) | `/volcano/`, `/province/` |
| Base URL (Interno) | `/api/v1/volcanoes/` |
| Base de Datos | Esquema `Volcano` |
| Política de Acceso Lectura | `Volcano.Read` |
| Dependencias | SQL Server, Redis, Geo API |

**Dominio:** Gestiona volcanes con referencia a la jerarquía geográfica (`ProvinceId`, `CantonId`, `DistrictId?`). Los datos de ubicación se enriquecen en tiempo real desde el Geo Service mediante llamadas HTTP concurrentes (`Task.WhenAll`).

**Endpoints expuestos:**

| Método | Ruta | Descripción | Autorización |
|---|---|---|---|
| GET | `/api/v1/volcanoes/` | Lista todos los volcanes | `Volcano.Read` |
| GET | `/api/v1/volcanoes/{id}` | Volcán por ID | `Volcano.Read` |
| GET | `/api/v1/volcanoes/province/{provinceId}` | Volcanes por provincia | `Volcano.Read` |

---

## 6. Arquitectura Interna — Clean Architecture

Cada microservicio está dividido en cuatro proyectos siguiendo las reglas de dependencia de Clean Architecture:

```
┌────────────────────────────────────────────────────────┐
│                     API Layer                          │
│  Program.cs · EndpointExtensions · Handlers · Profiles │
│            (Minimal API, AutoMapper, OpenAPI)           │
└──────────────────────────┬─────────────────────────────┘
                           │ depende de
┌──────────────────────────▼─────────────────────────────┐
│                 Application Layer                       │
│         DTOs · Interfaces · Services · Cache            │
│          (Lógica de negocio, sin dependencias externas) │
└──────────────────────────┬─────────────────────────────┘
                           │ depende de
┌──────────────────────────▼─────────────────────────────┐
│                   Domain Layer                          │
│              Models · Repository Interfaces             │
│              (Entidades puras, sin dependencias)        │
└─────────────────────────────────────────────────────────┘
                           ▲ implementado por
┌──────────────────────────┴─────────────────────────────┐
│                Infrastructure Layer                     │
│   DbContext · EntityConfigurations · Repositories       │
│   (EF Core, SQL Server, acceso a datos concreto)        │
└─────────────────────────────────────────────────────────┘
```

### Flujo de una petición HTTP

```
HTTP Request
     │
     ▼
[API Layer] Handler recibe la petición, la valida superficialmente
     │  invoca
     ▼
[Application Layer] IService → lógica de negocio, mapeo DTO
     │  si hay caché disponible → CacheService (Decorator)
     │  si no → Repository
     ▼
[Domain Layer] Entidades puras, reglas de negocio
     │
     ▼
[Infrastructure Layer] EF Core → Azure SQL Server
     │
     ▼
HTTP Response (Result<T> → IResult via ToResult())
```

### Patrón Decorator para Caché

```csharp
// Source Generator registra:
services.AddTransient<IBeachService, BeachService>();
services.Decorate<IBeachService, CacheBeachService>();

// CacheBeachService envuelve a BeachService:
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

## 7. Modelo de Datos

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

### 7.3 Geo (Jerarquía Administrativa)

```
Geo.Province
├── Id    INT PK
└── Name  NVARCHAR NOT NULL
   │
   └──< Geo.Canton
        ├── Id          INT      ┐ PK Compuesta
        ├── ProvinceId  INT      ┘ FK → Province
        ├── Name        NVARCHAR NOT NULL
        │
        └──< Geo.District
             ├── Id              INT  ┐
             ├── CantonId        INT  │ PK Compuesta
             ├── CantonProvinceId INT ┘ FK → Canton
             └── Name            NVARCHAR NOT NULL
```

### 7.4 Volcano

```
Volcano.Volcano
├── Id          INT IDENTITY PK
├── ProvinceId  INT NOT NULL   (referencia lógica a Geo.Province)
├── CantonId    INT NOT NULL   (referencia lógica a Geo.Canton)
├── DistrictId  INT NULL       (referencia lógica a Geo.District)
├── Name        NVARCHAR NOT NULL
└── Description NVARCHAR NOT NULL
```

> Las referencias entre Volcano y Geo son **lógicas** (sin FK físicas entre esquemas). La resolución se hace en tiempo de consulta mediante llamadas al Geo API.

---

## 8. Comunicación entre Servicios

### 8.1 Volcano → Geo (via Refit + Service Discovery)

El Volcano Service enriquece los datos de ubicación consultando el Geo Service usando un cliente Refit tipado (`IGeoDiscoverCostaRica`), registrado automáticamente en `ServiceDefaults`.

```
Volcano API
    │
    ├── Para cada volcán: GetProvinceById + GetCantonById + GetDistrictById
    │   (ejecutados CONCURRENTEMENTE con Task.WhenAll)
    │
    └── Geo API (/api/v1/geo/...)
```

El servicio de descubrimiento (Aspire Service Discovery) resuelve `https://geoservice` a la dirección real del contenedor en tiempo de ejecución.

### 8.2 Autenticación Servicio-a-Servicio

Las llamadas entre servicios usan `DiscoverCostaRicaAuthHandler` (un `DelegatingHandler`) que adjunta automáticamente un token Bearer de Entra ID usando `DiscoverCostaRicaTokenAcquisitionService`.

```csharp
// Configuración en ServiceDefaults
services.AddRefitClient<IGeoDiscoverCostaRica>()
    .ConfigureHttpClient(http => http.BaseAddress = new Uri("https://geoservice/api/v1/geo"))
    .AddHttpMessageHandler<DiscoverCostaRicaAuthHandler>()
    .AddStandardResilienceHandler();  // ← reintentos, circuit breaker, timeouts
```

### 8.3 Resiliencia

Todos los clientes HTTP usan `AddStandardResilienceHandler()` de .NET Resilience que configura automáticamente:
- **Reintentos** con backoff exponencial
- **Circuit Breaker** para aislar fallos
- **Timeout** por petición y total

---

## 9. Seguridad y Autenticación

### 9.1 Autenticación

Todos los servicios validan **JWT Bearer tokens** emitidos por **Microsoft Entra ID**. La configuración se centraliza en `ServiceDefaults.AddEntraIdAuthentication()`.

```
Cliente → [Bearer Token] → Servicio → [Valida con Entra ID JWKS]
```

Parámetros configurables por entorno (inyectados por Aspire):
- `EntraId__Audience` — App ID URI del cliente registrado
- `EntraId__Instance` — Endpoint de la instancia de Entra ID
- `EntraId__ClientId` — Client ID de la aplicación
- `EntraId__TenantId` — Tenant ID de Azure

### 9.2 Autorización — Políticas por Ámbito

Las políticas se generan automáticamente mediante Source Generators a partir de `DiscoverPolicies.cs`:

```csharp
[AuthorizationPolicy("Beaches.Read",  "Beaches.Read")]
[AuthorizationPolicy("Volcano.Read",  "Volcano.Read")]
[AuthorizationPolicy("Culture.Read",  "Culture.Read")]
[AuthorizationPolicy("Geo.Read",      "Geo.Read")]
public class DiscoverPolicies { }
```

El generator produce `AddPolicies()` que verifica el claim `roles` del token JWT. Las políticas se aplican a los endpoints con `.RequireAuthorization("Beaches.Read")`.

### 9.3 Roles

| Rol | Descripción |
|---|---|
| `Administrator` | Acceso total, incluyendo operaciones destructivas |
| `Writer` | Puede crear y modificar recursos |
| `Reader` | Solo lectura |
| `User` | Usuario autenticado sin permisos especiales |

### 9.4 Claims Personalizados

`ICurrentUserService` expone el contexto del usuario autenticado (implementado sobre `IHttpContextAccessor`), permitiendo acceder al usuario actual en cualquier capa de la aplicación.

---

## 10. Caché y Rendimiento

### 10.1 Redis como Caché Distribuida

Se usa **Redis** (integrado via `Aspire.StackExchange.Redis`) como caché distribuida. La interfaz `ICacheService` abstrae las operaciones de caché:

```csharp
public interface ICacheService
{
    Task<T?> Get<T>(string key, CancellationToken cancellationToken);
    Task Set<T>(string key, T value, CancellationToken cancellationToken);
}
```

### 10.2 Claves de Caché

| Clave | Dato Cacheado |
|---|---|
| `Geo.Provinces` | Lista completa de provincias |
| `Beach.Beaches` | Lista completa de playas |
| `Culture.Dishes` | Lista completa de platos |
| `Culture.Traditions` | Lista completa de tradiciones |
| `Volcano.Volcanos` | Lista completa de volcanes |

### 10.3 Estrategia de Invalidación

Actualmente el caché se invalida por TTL (tiempo de expiración definido en la configuración de Redis). Las operaciones de escritura no invalidan el caché explícitamente en la versión actual.

---

## 11. Observabilidad

### 11.1 OpenTelemetry

Todos los servicios tienen instrumentación OpenTelemetry configurada en `ServiceDefaults`:

| Signal | Instrumentación |
|---|---|
| **Trazas** | ASP.NET Core + HTTP Client (excluye `/health` y `/alive`) |
| **Métricas** | ASP.NET Core + HTTP Client + Runtime |
| **Logs** | OpenTelemetry Logging con scopes y mensaje formateado |

El exportador se configura vía variable de entorno `OTEL_EXPORTER_OTLP_ENDPOINT` (compatible con Aspire Dashboard y servicios externos).

### 11.2 Aspire Dashboard

En entorno de desarrollo, el Aspire Dashboard provee:
- Visualización de trazas distribuidas
- Métricas en tiempo real
- Logs estructurados correlacionados por TraceId

### 11.3 Health Checks

Cada servicio expone (solo en Development):
- `GET /health` — todos los checks deben pasar para considerar el servicio listo
- `GET /alive` — solo checks con tag `live`, para liveness probe de Kubernetes/Container Apps

### 11.4 Logging Personalizado (MongoDB)

`DiscoverCostaRicaLoggerProvider` + `DiscoverCostaRicaLogger` proveen un logger que persiste entradas en **MongoDB**, permitiendo consultar logs de aplicación desde la base de datos.

Estructura de una entrada de log (`LogEntryModel`):
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

## 12. Generadores de Código Fuente

La solución usa dos Roslyn IIncrementalGenerator registrados como analizadores:

### 12.1 ServiceRegistrationGenerator

Escanea el assembly en busca de clases con los atributos de lifetime y genera:

```csharp
// Generado automáticamente en compilación:
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

| Atributo | Lifetime | Comportamiento |
|---|---|---|
| `[TransientService]` | Transient | Registra `services.AddTransient<IFoo, Foo>()` |
| `[ScopedService]` | Scoped | Registra `services.AddScoped<IFoo, Foo>()` |
| `[SingletonService]` | Singleton | Registra `services.AddSingleton<IFoo, Foo>()` |
| `[DecoratorService]` | — | Registra con Scrutor `services.Decorate<IFoo, DecoratorFoo>()` |

### 12.2 AuthorizationPolicyGenerator

Escanea clases decoradas con `[AuthorizationPolicy(policyName, scope)]` y genera el método `AddPolicies()` para registrar políticas de autorización basadas en claims de ámbito:

```csharp
// Generado automáticamente:
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

## 13. Infraestructura y Despliegue

### 13.1 Azure (Producción)

| Recurso | Servicio Azure |
|---|---|
| Microservicios (×4) | Azure Container Apps |
| Base de Datos | Azure SQL Server (existente, referenciado como parámetro) |
| Caché | Azure Cache for Redis (connection string externo) |
| Logs | Azure Cosmos DB con API MongoDB (connection string externo) |
| Identidad | Microsoft Entra ID |
| Orchestración deploy | Azure Developer CLI (`azd`) |

El archivo `azure.yaml` declara los 4 servicios como Container Apps. Los parámetros sensibles (credenciales de Entra ID) se inyectan como parámetros secretos de Aspire en tiempo de despliegue.

### 13.2 Parámetros de Entorno

| Parámetro | Descripción |
|---|---|
| `EntraId__Audience` | App ID URI |
| `EntraId__Instance` | `https://login.microsoftonline.com/` |
| `EntraId__ClientId` | Application (Client) ID |
| `EntraId__TenantId` | Directory (Tenant) ID |
| `Azure__TenantId` | Tenant para adquisición de tokens entre servicios |
| `Azure__ClientId` | Client ID para tokens entre servicios |
| `Azure__ClientSecret` | Client Secret (secreto) |
| `Azure__Scope` | Scope para tokens entre servicios |
| `existingSqlServerName` | Nombre del SQL Server Azure existente |
| `existingSqlServerResourceGroup` | Resource Group del SQL Server |

### 13.3 Desarrollo Local con .NET Aspire

```bash
dotnet run --project DiscoverCostaRica.AppHost
# o
make run
```

El AppHost levanta todos los servicios con sus dependencias inyectadas automáticamente. El Aspire Dashboard se disponibiliza en `http://localhost:18888`.

### 13.4 Desarrollo Local con Docker Compose

```bash
docker compose up
```

Levanta los 4 microservicios + SQL Server 2022 + Redis 7 + MongoDB 7, sin necesidad de .NET Aspire.

| Servicio | Puerto |
|---|---|
| beaches-api | 7000 |
| culture-api | 7001 |
| volcano-api | 7002 |
| geo-api | 7003 |
| SQL Server | 1433 |
| Redis | 6379 |
| MongoDB | 27017 |

---

## 14. Versionamiento de API

Las rutas siguen el patrón `/api/v{version}/{resource}` (ejemplo: `/api/v1/beaches/`).

La versión se negocia via URL path. La configuración en `ServiceDefaults`:

```csharp
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;  // ← devuelve header api-supported-versions
});
```

**Versiones declaradas:** v1.0, v2.0 (v2.0 definida pero no implementada aún).

**Política de deprecación:** Una versión se soporta mínimo 12 meses después de que la versión siguiente sea lanzada en producción.

---

## 15. Patrones de Respuesta

### 15.1 Result Pattern

Todas las operaciones de negocio devuelven `Result<T>`, un discriminated union de tres variantes:

```csharp
// Respuesta genérica
record Result(int StatusCode, string? Message);

// Éxito
sealed record Success(object Value, int StatusCode = 200) : Result;

// Error
sealed record Failure(string Message, int StatusCode = 500) : Result;

// Result tipado — combina las anteriores mediante conversión implícita
sealed record Result<TResult>(int StatusCode, string? Message) : Result
{
    TResult? Value { get; set; }
    static implicit operator Result<TResult>(Success s) => ...;
    static implicit operator Result<TResult>(Failure f) => ...;
}
```

### 15.2 Conversión a IResult (HTTP)

El método de extensión `ToResult()` convierte `Result<T>` al `IResult` de Minimal APIs:

| StatusCode | Respuesta HTTP |
|---|---|
| 200 | `Results.Ok(result)` |
| 404 | `Results.NotFound(result)` |
| 400 | `Results.BadRequest(result)` |
| 500 | `Results.InternalServerError()` |
| Otro | `Results.Problem(message, statusCode)` |

### 15.3 Manejo Global de Excepciones

`GlobalExceptionHandler` (registrado en todos los servicios) captura excepciones no manejadas y devuelve un `ProblemDetails` con estado 500, sin exponer detalles internos al cliente.

### 15.4 Documentación OpenAPI

Cada servicio expone su especificación OpenAPI en `/openapi/v1.json` y una interfaz interactiva **Scalar** en `/docs`.

---

## 16. Diagrama de Componentes

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         DiscoverCostaRica.Shared                        │
│  AuthConstants · Roles · Scopes · Policies · CacheKeys · RoutesConstants│
│  Result<T> · Success · Failure · ICacheService · IGeoDiscoverCostaRica   │
│  [TransientService] · [DecoratorService] · [AuthorizationPolicy]        │
└─────────────────────────────────────────────────────────────────────────┘
         ▲ referenciado por todos los proyectos

┌─────────────────────────────────────────────────────────────────────────┐
│                      DiscoverCostaRica.ServiceDefaults                  │
│  AddServiceDefaults() · AddEntraIdAuthentication() · AddVersioning()    │
│  ConfigureOpenTelemetry() · GlobalExceptionHandler                      │
│  DiscoverCostaRicaAuthHandler (servicio-a-servicio)                     │
└─────────────────────────────────────────────────────────────────────────┘
         ▲ referenciado por todos los servicios API

┌─────────────────────────────────────────────────────────────────────────┐
│                   DiscoverCostaRica.SourceGenerators                    │
│  ServiceRegistrationGenerator  →  genera AddGeneratedServices_*()      │
│  AuthorizationPolicyGenerator  →  genera AddPolicies()                  │
└─────────────────────────────────────────────────────────────────────────┘
         ▲ Analyzer en Application e Infrastructure layers

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
│  │  GeoService,       │  │  │  │  llama a Geo API)  │  │
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
         │  (datos frecuentes) │
         └─────────────────────┘
```

---

## Referencias

| Recurso | Enlace / Ubicación |
|---|---|
| .NET Aspire | `https://learn.microsoft.com/dotnet/aspire` |
| YARP Reverse Proxy | `https://microsoft.github.io/reverse-proxy/` |
| Refit HTTP Client | `https://github.com/reactiveui/refit` |
| Scrutor (Decorator DI) | `https://github.com/khellang/Scrutor` |
| Asp.Versioning | `https://github.com/dotnet/aspnet-api-versioning` |
| Scalar OpenAPI UI | `https://scalar.com` |
| Roslyn Source Generators | `DiscoverCostaRica.SourceGenerators/` |
| Configuración Aspire | `DiscoverCostaRica.AppHost/AppHost.cs` |
| Defaults compartidos | `DiscoverCostaRica.ServiceDefaults/Extensions.cs` |
| Rutas de API | `DiscoverCostaRica.Shared/Routes/RoutesConstants.cs` |
| Políticas de autorización | `DiscoverCostaRica.Shared/Authentication/DiscoverPolicies.cs` |
| Despliegue Azure | `azure.yaml` |
