# Feature Specification: Platform Cross-Cutting Concerns (Gateway, Security, Cache, Observability)

**Feature Branch**: `005-platform-cross-cutting` (baseline, reverse-engineered)

**Created**: 2026-10-14

**Status**: Baseline (as-built) — describes `DiscoverCostaRica.AppHost`, `ServiceDefaults`,
`Shared`, `SourceGenerators`, CI

**Input**: Reverse engineering of the platform layer that every service shares.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Single public entry point (Priority: P1)

An API consumer calls one host and short public paths (`/provinces`, `/beaches`, ...)
without knowing the internal services.

**Acceptance Scenarios**:

1. **Given** the gateway runs, **When** `/provinces` is called, **Then** it is forwarded to
   Geo `/api/v1/geo/provinces`.
2. **Given** an unknown path, **Then** (as built) it is forwarded to Volcano. See PLT-D1.

As-built gateway table (prefix transform = `/api/v1/<group>` + the original path):

| Public | Forwarded to | Works? |
|---|---|---|
| `/provinces/{**}` | Geo `/api/v1/geo/provinces/...` | ✅ (by-id fails, GEO-D1) |
| `/canton/{**}` | Geo `/api/v1/geo/canton/...` | ❌ service route is `cantons` |
| `/districts/{**}` | Geo `/api/v1/geo/districts/...` | ✅ |
| `/beaches/{**}` | Beaches `/api/v1/beaches/beaches...` | ❌ BCH-D1 |
| `/tradition/{**}` | Culture `/api/v1/traditions/tradition` | ✅ |
| `/dish/{**}` | Culture `/api/v1/traditions/dish` | ✅ |
| `/volcano/{**}` | Volcano `/api/v1/volcanoes/volcano...` | ❌ VOL-D7 |
| `/province/{**}` | Volcano `/api/v1/volcanoes/province/...` | ⚠️ VOL-D1 |
| `/{**}` | Volcano `/api/v1/volcanoes/...` | ⚠️ catch-all |

### User Story 2 - Secured service-to-service and client access (Priority: P1)

Only callers holding the correct Entra ID **app role** can read each catalog.

**Acceptance Scenarios**:

1. **Given** no bearer token, **Then** 401.
2. **Given** a token without the needed role (`Geo.Read`, `Beaches.Read`, `Culture.Read`,
   `Volcano.Read`), **Then** 403.
3. **Given** Volcano calls Geo, **Then** it gets a client-credentials token for the
   configured scope and sends it as a bearer token.

Policies are generated at compile time from `[AuthorizationPolicy]` attributes. They check
the `roles` claim, despite being named "Scopes" in code.

### User Story 3 - Operate and observe (Priority: P2)

An operator sees traces, metrics and logs and checks health.

**Acceptance Scenarios**:

1. OpenTelemetry traces and metrics (ASP.NET Core, HttpClient, runtime) are exported over
   OTLP when `OTEL_EXPORTER_OTLP_ENDPOINT` is set.
2. `/health` and `/alive` exist **only in Development**.
3. Errors are also written to MongoDB (`discovercotarica.logs` — sic) by a custom logger
   provider. It handles only the Error level, is fire-and-forget, and fakes an exception
   object when none was given.
4. Unhandled exceptions return 500 ProblemDetails ("An unexpected error ocurred" — sic).

### User Story 4 - Developer experience (Priority: P3)

1. API docs: OpenAPI at `/openapi/v1.json` and the Scalar UI at `/docs` in **all** environments.
2. DI registration is generated from `[TransientService]`, `[ScopedService]`,
   `[SingletonService]` and `[DecoratorService]` (the decorator uses Scrutor).
3. Local run: `dotnet run` the Aspire AppHost with an existing Azure SQL server, plus Redis
   and MongoDB connection strings given as parameters.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Gateway MUST expose the public routes above.
- **FR-002**: Every catalog endpoint MUST enforce its app-role policy.
- **FR-003**: Service-to-service calls MUST carry a client-credentials token and use the
  standard resilience pipeline (retry, circuit breaker, timeout) and service discovery.
- **FR-004**: List responses MUST be cache-aside in Redis.
- **FR-005**: All services MUST share one SQL database, with one schema-prefixed table set
  per service (as built: tables named `"Geo.Province"`, which is a dotted table name in the
  `dbo` schema, not a real schema).
- **FR-006**: Response envelope v1: `{ statusCode, message?, value? }`. A 500 from the
  presenter has an empty body. Exceptions produce ProblemDetails.
- **FR-007**: API versioning via URL segment `v{version}`. Versions 1.0 and 2.0 are
  declared, only 1.0 is mapped, and `api-supported-versions` is reported.

### Known defects / risks found during reverse engineering

- **PLT-D1 (high)**: The gateway catch-all `/{**}` sends unknown traffic to Volcano.
- **PLT-D2 (high)**: Shared DB, "schemas" are only dotted table names, and every service uses
  the same connection identity, so service data ownership is not enforced.
- **PLT-D3 (high)**: `Shared` mixes domain DTOs, ASP.NET, Redis, MongoDB, Refit and Azure.Identity.
  Every service's Application layer therefore depends on web and infrastructure packages,
  which breaks the Dependency Rule.
- **PLT-D4 (high)**: Application services return HTTP status codes (`StatusCodes` from
  `Microsoft.AspNetCore.Http`).
- **PLT-D5 (medium)**: `ServiceDefaults` registers Mongo, Redis and the Geo Refit client in
  **every** service, even services that do not use them.
- **PLT-D6 (medium)**: Health endpoints are dev-only, so Container Apps probes cannot use them.
- **PLT-D7 (medium)**: Package version drift (EF Core 9.0.9 / 9.0.10 / 10.0.1, AutoMapper
  15 / 16, OpenApi `10.0.0-rc.2`) and unused EF6 / Dapr leftovers.
- **PLT-D8 (high)**: CI only deploys. No build, test or analyzer gate.
- **PLT-D9 (medium)**: The only test is one integration test that needs real Azure secrets.
- **PLT-D10 (low)**: `RequireHttpsMetadata = false` on JWT validation.
- **PLT-D11 (low)**: Typos in public identifiers (`CachedProviceService`, `DaprCompoonents`,
  `ResourceParemeterBuilder`, `AddGlobalExeption`, `DiscoverCostaRica.CultureSerice.Api.http`).
- **PLT-D12 (medium)**: AutoMapper ≥ 15 has a commercial licence. Mapping profiles live in the
  Api layer and map Domain → Application DTOs, so the outer layer knows the inner model
  shapes on the wrong side.

## Success Criteria *(mandatory)*

- **SC-001**: 100% of public gateway routes reach a valid endpoint, and unknown paths return 404.
- **SC-002**: The Dependency Rule is verified by automated tests in every service.
- **SC-003**: Every merge to `main` passes build, tests and analyzers before deploy.

## Assumptions

- Entra ID app registrations and app roles already exist. Secrets come from `azd`
  parameters and environment variables.
- Production runs on Azure Container Apps through `azd`.
