# Implementation Plan: Clean Architecture Migration

**Branch**: `006-clean-architecture-migration` | **Date**: 2026-10-14 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/006-clean-architecture-migration/spec.md`

## Summary

Restructure the four catalog services and the shared platform to Uncle Bob's Clean
Architecture, under a frozen v1 HTTP contract that is pinned first by characterization
tests. Replace the monolithic `Shared` library with components split by reason to change.
Move persistence and transport behind ports owned by the Use Case layer, and enforce the
Dependency Rule with architecture tests in CI. On the way, fix the defects catalogued in
specs 001–005 (GEO-D2..D5, VOL-D1/D2/D4..D8, BCH-D1..D2, CUL-D1..D3, PLT-D1..D13; GEO-D1 and VOL-D3 were withdrawn by the characterization tests).

## Technical Context

**Language/Version**: C# (latest) on .NET 10 LTS

**Primary Dependencies**: ASP.NET Core Minimal APIs, Asp.Versioning, EF Core 10 (SQL Server),
`Microsoft.Extensions.Caching.Hybrid` with the Redis L2, Refit + `Microsoft.Extensions.Http.Resilience`,
Microsoft.Identity.Web, .NET Aspire 13, YARP, Scrutor (decorators), in-repo Roslyn
source generators

**Storage**: Azure SQL (one schema per service), Redis (L2 cache)

**Testing**: xUnit v3, NetArchTest.Rules, Testcontainers (MsSql, Redis), WireMock.Net,
`Microsoft.AspNetCore.Mvc.Testing`, Aspire.Hosting.Testing, coverlet

**Target Platform**: Linux containers on Azure Container Apps (`azd`)

**Project Type**: Distributed web services (microservices + gateway)

**Performance Goals**: cached reads p95 < 50 ms; uncached volcano list (N = 50) p95 < 300 ms

**Constraints**: no breaking v1 changes (FR-004); ≤ 1 Geo call per volcano list (SC-003);
no pre-release packages

**Scale/Scope**: 4 services, about 12 endpoints, about 600 reference rows; read-only

## Constitution Check

*Gate: must pass before Phase 0 research. Re-checked after Phase 1 design.*

| Gate | Status | Evidence |
|---|---|---|
| G1 Dependency Rule enforced | PASS (planned) | `tests/DiscoverCostaRica.ArchitectureTests` (T010–T013), CI gate T006 |
| G2 Persistence-ignorant domain | PASS | data-model.md: private setters, factories, value-object keys; EF config only in Infrastructure |
| G3 One interactor per use case, ports in Application | PASS | Structure below: `UseCases/<Name>/`, `Ports/` |
| G4 HTTP only in adapters | PASS | `Result<T>` + `Error` with no status codes; `V1EnvelopePresenter` in `BuildingBlocks.Web` |
| G5 Data ownership + contracts | PASS (US6) | R9 schemas; `Contracts.Geo` is the only cross-service reference |
| G6 Test-first | PASS | Characterization tests first (T014–T018), then unit, integration, contract |
| G7 OTel, TTL, resilience | PASS | R4 HybridCache TTL; MongoDB logger removed (US7); standard resilience on Refit |
| G8 Complexity tracked | PASS | See Complexity Tracking |

Post-design re-check: **PASS**. No gate violations introduced by data-model.md or contracts/.

## Project Structure

### Documentation (this feature)

```text
specs/006-clean-architecture-migration/
├── spec.md
├── plan.md              # this file
├── research.md          # R1–R10 decisions
├── data-model.md        # entities, value objects, ports
├── quickstart.md        # how to verify each story
├── contracts/
│   ├── public-api-v1.md         # frozen v1 contract (+ additive routes)
│   └── geo-locations-resolve.md # internal batch contract
├── checklists/requirements.md
└── tasks.md
```

### Source Code (target)

```text
src/
├── BuildingBlocks/
│   ├── DiscoverCostaRica.BuildingBlocks.Application/     # Result<T>, Error, IUseCase<,>, DI attributes (BCL only)
│   ├── DiscoverCostaRica.BuildingBlocks.Web/             # V1EnvelopePresenter, versioning, auth policies, endpoint helpers
│   └── DiscoverCostaRica.BuildingBlocks.Infrastructure/  # Entra token provider, EF conventions, HybridCache setup
├── Contracts/
│   └── DiscoverCostaRica.Contracts.Geo/                  # LocationCode, ResolvedLocation, Geo DTOs (BCL only)
├── DiscoverCostaRica.Geo/
│   ├── DiscoverCostaRica.Geo.Domain/                     # ENTITIES
│   │   ├── Province.cs  Canton.cs  District.cs
│   │   └── ValueObjects/ ProvinceCode.cs CantonCode.cs DistrictCode.cs
│   ├── DiscoverCostaRica.Geo.Application/                # USE CASES
│   │   ├── Ports/ IGeoReadRepository.cs
│   │   ├── UseCases/
│   │   │   ├── GetProvinces/        GetProvincesInteractor.cs  GetProvincesRequest.cs  ProvinceResponse.cs
│   │   │   ├── GetProvinceById/
│   │   │   ├── GetCantonsByProvince/
│   │   │   ├── GetCantonById/
│   │   │   ├── GetDistrictsByCanton/
│   │   │   ├── GetDistrictById/
│   │   │   └── ResolveLocations/
│   │   └── Caching/ CachedGetProvinces.cs                # decorator (HybridCache abstraction)
│   ├── DiscoverCostaRica.Geo.Infrastructure/             # FRAMEWORKS & DRIVERS (gateways)
│   │   ├── Persistence/ GeoDbContext.cs  Configurations/  Migrations/
│   │   └── Repositories/ GeoReadRepository.cs
│   └── DiscoverCostaRica.Geo.Api/                        # INTERFACE ADAPTERS + MAIN
│       ├── Program.cs                                    # composition root (Main)
│       └── Endpoints/ ProvinceEndpoints.cs CantonEndpoints.cs DistrictEndpoints.cs LocationEndpoints.cs
├── DiscoverCostaRica.Volcano/ (same shape)
│   ├── ...Application/Ports/ IVolcanoReadRepository.cs  IGeoLocationGateway.cs
│   └── ...Infrastructure/Geo/ GeoLocationGateway.cs  IGeoApi.cs (Refit)   # adapter over Contracts.Geo
├── DiscoverCostaRica.Beaches/ (same shape)
└── DiscoverCostaRica.Culture/ (same shape)

DiscoverCostaRica.AppHost/          # Aspire orchestration + YARP route table
DiscoverCostaRica.ServiceDefaults/  # Aspire defaults only (OTel, health, discovery, resilience)
DiscoverCostaRica.SourceGenerators/ # DI + policy generators (kept)

tests/
├── DiscoverCostaRica.ArchitectureTests/            # Dependency Rule fitness functions
├── DiscoverCostaRica.BuildingBlocks.UnitTests/     # presenter / Result
├── DiscoverCostaRica.Geo.UnitTests/                # entities + interactors with fakes
├── DiscoverCostaRica.Volcano.UnitTests/
├── DiscoverCostaRica.Beaches.UnitTests/
├── DiscoverCostaRica.Culture.UnitTests/
├── DiscoverCostaRica.CharacterizationTests/        # frozen v1 contract via WebApplicationFactory
├── DiscoverCostaRica.IntegrationTests/             # Testcontainers: EF gateways, migrations, HybridCache
└── DiscoverCostaRica.AppHost.Tests/                # gateway route table, end-to-end (existing Tests project, renamed)
```

**Structure Decision**: Keep the existing per-service folders and the four-project split, so
the git history stays readable. Uncle Bob's four circles map one-to-one: Domain = Entities,
Application = Use Cases, Api = Interface Adapters + Main, Infrastructure = Frameworks &
Drivers (gateway implementations). The root `DiscoverCostaRica.Shared` is dissolved into
`src/BuildingBlocks/*` and `src/Contracts/*`, then deleted.

### Dependency graph (target)

```text
Geo.Api ──► Geo.Application ──► Geo.Domain
   │             ▲    └──► BuildingBlocks.Application
   └──► Geo.Infrastructure ──┘
   └──► BuildingBlocks.Web ──► BuildingBlocks.Application

Volcano.Infrastructure ──► Contracts.Geo   (only cross-service edge)
```

## Migration Strategy (Strangler, one service at a time)

1. **Pin**: write characterization tests for the as-built v1 behaviour. Known-broken
   behaviour is pinned as it is today, tagged `[Trait("Defect", "…")]`, and rewritten when fixed.
2. **Foundation**: building blocks, contracts, central packages, architecture tests with a
   ratchet baseline of known violations.
3. **Geo** → **Volcano** → **Beaches** → **Culture**. Each migration deletes its lines from
   `architecture-baseline.txt`. A service is "done" when none of its lines remain.
4. **Gateway** route table (US5).
5. **Schemas** (US6) and **Ops** (US7).
6. Delete `DiscoverCostaRica.Shared`, update docs and ADRs.

## Risks

| Risk | Mitigation |
|---|---|
| Hidden consumers depend on the broken behaviour | Characterization tests + the defect list in the PR; deprecated routes kept with `Deprecation` headers |
| EF mapping of pure entities with composite keys | R3 fallback to persistence models (log it in Complexity Tracking) |
| Schema move in production | Reversible migration, maintenance window, backup first (quickstart §6) |
| No `dotnet` SDK in some agent sandboxes | CI is the source of truth; `global.json` pins the SDK |

## Complexity Tracking

| Addition | Why needed | Simpler alternative rejected because |
|---|---|---|
| `IUseCase<TReq,TRes>` generic boundary | One decorator (caching, logging) can wrap any interactor | Per-interactor interfaces would need N decorator classes |
| Batch resolve endpoint in Geo | Removes 3N fan-out (SC-003) | Full replication needs a broker (deferred) |
| 3 BuildingBlocks + 1 Contracts project | Component principles (CCP/CRP, SDP) | One Shared project is the defect PLT-D3 |
| Characterization test project | Safe refactor under a frozen contract | Unit tests alone do not pin the wire format |
| Scrutor allowed in `*.Application` | Generated DI code calls `Decorate<,>` for use-case decorators | Hand-written decorator wiring in every Main |
| Ratchet baseline instead of allow-list | Rules apply to every project from day one; fixes are locked in | A per-service allow-list hides new violations in a listed service |
