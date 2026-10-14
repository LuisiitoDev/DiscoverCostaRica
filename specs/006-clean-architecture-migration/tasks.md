# Tasks: Clean Architecture Migration

**Input**: Design documents from `/specs/006-clean-architecture-migration/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

**Tests**: REQUIRED (Constitution VII). Test tasks come before their implementation and
must fail first.

**Organization**: Grouped by user story. `[P]` = can run in parallel (different files, no
dependency). `[USn]` = user story.

---

## Phase 1: Setup (shared infrastructure)

**Purpose**: Reproducible builds and a CI gate. No behaviour change.

- [x] T001 Add `global.json` pinning the .NET 10 SDK (rollForward `latestFeature`) at repo root
- [x] T002 Add `Directory.Build.props` (Nullable, ImplicitUsings, TreatWarningsAsErrors, AnalysisLevel latest, code style in build, NuGet audit on all packages) at repo root
- [x] T003 Add `Directory.Packages.props` with Central Package Management; one version per package; EF Core 10.0.x, OpenApi 10.0.x (rc dropped); transitive security pins (SharpCompress, Snappier, MessagePack, System.Security.Cryptography.Xml). AutoMapper is removed later in T051
- [x] T004 [P] Remove unused packages: `EntityFramework` 6.5.1, `Microsoft.AspNetCore.Http` 2.3.0, `System.Linq.Async`, `Swashbuckle.AspNetCore`, `Microsoft.Build.Tasks.Core`, VS container tooling preview; delete `DaprCompoonents.cs`
- [x] T005 [P] Add `.editorconfig` (file-scoped namespaces, `_camelCase` private fields, no BOM) and fix typos: `CachedProviceService`→`CachedProvinceService`, `ResourceParemeterBuilder`→`ResourceParameterBuilder`, `AddGlobalExeption`→`AddGlobalException`, rename `DiscoverCostaRica.CultureSerice.Api.http`
- [x] T006 Add `.github/workflows/ci.yml` (restore → Release build with warnings as errors → all tests except `Category=RequiresAzure` → coverage + trx artifacts); `azure-dev.yml` calls `ci.yml` as a reusable workflow and deploys only when it passes
- [x] T007 [P] Add `.github/dependabot.yml` (nuget + github-actions, weekly)

**Checkpoint**: The solution builds with zero warnings, and CI runs on every PR.

---

## Phase 2: Foundational (blocks every story)

**Purpose**: Building blocks, contracts, test harnesses. ⚠️ No service migration until this is done.

- [x] T008 Create `src/BuildingBlocks/DiscoverCostaRica.BuildingBlocks.Application` (BCL only): `Error`, `ErrorType`, `Result<T>` (a class, so `default` cannot pose as success), `IUseCase<TRequest,TResponse>`; DI lifetime attributes moved here from `Shared/Attributes`
- [x] T009 Create `src/BuildingBlocks/DiscoverCostaRica.BuildingBlocks.Web`: `V1EnvelopePresenter` + `V1Envelope<T>`, routing constants, versioning, Entra ID authentication and app-role policies (moved from `Shared` and `ServiceDefaults`)
- [x] T010 [P] Create `src/BuildingBlocks/DiscoverCostaRica.BuildingBlocks.Infrastructure`: `IAccessTokenProvider` + singleton `ClientCredentialsTokenProvider` + `BearerTokenHandler` (replace the per-call credential), `AddCatalogDbContext<TInterface,TContext>()`, `AddCatalogHybridCache()` (Redis L2, 24 h TTL). The per-service `schema` parameter arrives with US6
- [x] T011 [P] Create `src/Contracts/DiscoverCostaRica.Contracts.Geo` (BCL only) with the DTOs from data-model §6
- [x] T012 Update `DiscoverCostaRica.SourceGenerators`: fully-qualified names so closed generics such as `IUseCase<,>` register and decorate correctly; diagnostic `DCR001` for a decorator without an interface; policies emitted only in the assembly that declares them; generator unit tests
- [x] T013 Create `tests/DiscoverCostaRica.ArchitectureTests` (NetArchTest.eNhancedEdition + .csproj graph). Instead of a per-service allow-list, known violations live in a **ratchet baseline** (`architecture-baseline.txt`): new violations fail, and fixed-but-listed ones fail too, so the file can only shrink
- [x] T014 Create `tests/DiscoverCostaRica.CharacterizationTests`: one `WebApplicationFactory` per service (marker interface per Api), Production environment, real SQL Server (Testcontainers) built from the services' own migrations and seeded with plain SQL, fake `roles` auth, and every infrastructure seam replaced in one file (`InfrastructureSeams.cs`); Volcano calls an in-process Geo
- [x] T015 [P] Characterization tests, Geo v1: every row of contracts § Geo, as built. Known defects are pinned to their **current** behaviour and tagged `[Trait("Defect", "GEO-Dn")]`, then rewritten when fixed
- [x] T016 [P] Characterization tests, Beaches v1
- [x] T017 [P] Characterization tests, Culture v1
- [x] T018 [P] Characterization tests, Volcano v1 (VOL-D1/D2/D4/D5 pinned as built and tagged)
- [x] T019 Create `tests/DiscoverCostaRica.IntegrationTests` with `SqlServerFixture` and `RedisFixture` (shared in `tests/DiscoverCostaRica.Testing`): migrations apply with no pending model changes; HybridCache shares entries through Redis
- [x] T020 [P] Unit tests in `tests/DiscoverCostaRica.BuildingBlocks.UnitTests`: `Result<T>`, `V1EnvelopePresenter` (byte-equal with the as-built envelope), HybridCache defaults, source generators

**Checkpoint**: Contract pinned, fitness functions running, building blocks ready.

---

## Phase 3: User Story 1 — The architecture defends itself (P1) 🎯 MVP

**Goal**: Violations fail the build. **Independent test**: quickstart §2.

- [x] T021 [US1] Rules `no-cross-service-references` and `shared-components-independent-of-services` (read from .csproj, so unused references are caught too)
- [x] T022 [US1] Rules `application-allowed-references` (.csproj) and `application-no-framework-types` (IL): no ASP.NET Core, EF Core, Redis, Refit, MongoDB, Azure SDK or AutoMapper in `*.Application`
- [x] T023 [US1] Rules `domain-references-nothing` and `inner-circles-bcl-only` (Domain, BuildingBlocks.Application and Contracts depend on `System.*` only)
- [x] T024 [US1] Rules `ports-are-interfaces`, `use-cases-in-application`, `infrastructure-allowed-references`, `pure-components-reference-nothing`, `no-legacy-shared`
- [x] T025 [US1] Document the rules and the ratchet in `tests/DiscoverCostaRica.ArchitectureTests/README.md`

**Checkpoint**: US1 is delivered. Every following story deletes its lines from `architecture-baseline.txt`.

---

## Phase 4: User Story 2 — Geo correct and clean (P1)

**Goal**: Geo on Clean Architecture; GEO-D2..D5 fixed (GEO-D1 was withdrawn, see spec 001). **Independent test**: quickstart §3.

### Tests first

- [x] T026 [P] [US2] Entity tests: `ProvinceCode` range, `CantonCode` / `DistrictCode` equality, blank names rejected, in `tests/DiscoverCostaRica.Geo.UnitTests/Domain/`
- [x] T027 [P] [US2] Interactor tests with `FakeGeoReadRepository` for all 7 use cases (not found messages per contract), in `tests/DiscoverCostaRica.Geo.UnitTests/UseCases/`
- [x] T028 [P] [US2] Integration tests: `GeoReadRepository` against MsSql; same canton id in two provinces must not leak (GEO-D2, D3); set-based `ResolveAsync` — covered by the characterization suite, which runs every repository query against SQL Server
- [x] T029 [US2] Rewrite the GEO-D2/D3/D4 characterization tests to the fixed behaviour; add tests for the hierarchical and deprecated routes (`Deprecation` + `Link` headers)

### Implementation

- [x] T030 [US2] Domain: `Province`, `Canton`, `District` and the value objects in `src/DiscoverCostaRica.Geo/DiscoverCostaRica.Geo.Domain/` (remove the navigation-heavy `*Model` classes)
- [x] T031 [US2] Application: `Ports/IGeoReadRepository.cs`, plus `UseCases/*` interactors with Request/Response models and static `From()` mappers; delete `GeoService`, `ProvinceService` and `Interfaces/`
- [x] T032 [US2] Application: `Caching/CachedGetProvinces` decorator using `HybridCache` (key `geo:provinces`, TTL 24 h)
- [x] T033 [US2] Infrastructure: rewrite `GeoReadRepository` (scoped district queries; drop the token passed inside the `FindAsync` key array, harmless but misleading); EF configurations map entities and value objects; move `IGeoContext` to an internal type
- [x] T034 [US2] Api: `Endpoints/*` controllers → interactor → `V1EnvelopePresenter`; add the hierarchical routes and `POST locations:resolve` (validation 1..200); delete `Profiles/MappingProfile.cs` and `Handler/GeoHandler.cs`
- [x] T035 [US2] Composition root `Program.cs`: register via generated DI, HybridCache, DbContext; remove the AutoMapper and Mongo registrations
- [x] T036 [US2] Delete the Geo lines from `architecture-baseline.txt`; all Geo tests green

**Checkpoint**: Geo fully migrated; v1 contract intact; GEO-D2..D5 closed.

---

## Phase 5: User Story 3 — Volcano efficient and resilient (P1)

**Goal**: 1 Geo call, stale-on-error, routes fixed. **Depends on**: Phase 4 (batch endpoint).
**Independent test**: quickstart §4.

### Tests first

- [x] T037 [P] [US3] Interactor tests with `FakeVolcanoReadRepository` + `FakeGeoLocationGateway`: list, by id and by province all include location; codes deduplicated before the gateway call
- [x] T038 [P] [US3] Contract tests: `GeoLocationGateway` against a WireMock.Net stub using JSON fixtures shared with the Geo provider test (`tests/fixtures/geo-resolve/*.json`) — the gateway is exercised over real HTTP against the in-process Geo; edge cases with a fake handler in `Volcano.UnitTests`
- [x] T039 [P] [US3] Integration tests: exactly 1 resolve call for 50 volcanoes; stub down → stale locations served; never resolved → no `location`, still 200
- [x] T040 [US3] Rewrite the VOL-D1/D2/D5 characterization tests to the fixed behaviour; add a 405 test for POST/DELETE

### Implementation

- [x] T041 [US3] Domain: `Volcano` entity + `LocationCode` VO; remove `VolcanoModel` and the mixed `VolcanoService.*` namespaces
- [x] T042 [US3] Application: `Ports/IVolcanoReadRepository`, `Ports/IGeoLocationGateway`; interactors `GetVolcanoes`, `GetVolcanoById`, `GetVolcanoesByProvince`; delete `LocationService`, `MapperVolcanoService`, `Constants/ApiRoutes.cs`
- [x] T043 [US3] Application: `CachedGetVolcanoes` decorator (TTL 1 h)
- [x] T044 [US3] Infrastructure: `Geo/IGeoApi` (Refit, `Contracts.Geo` DTOs) + `GeoLocationGateway` (per-code HybridCache, stale-on-error, standard resilience handler); remove `IGeoDiscoverCostaRica` from Shared and its global registration in ServiceDefaults (PLT-D5)
- [x] T045 [US3] Api: `MapGet` endpoints with path binding (`{id:int}`, `{provinceId:int}`) → presenter; delete `VolcanoHandler`, `MappingProfile`
- [x] T046 [US3] Delete the Volcano lines from `architecture-baseline.txt`; all Volcano tests green

**Checkpoint**: VOL-D1, D2, D4..D6, D8 closed (VOL-D3 withdrawn). SC-003 measured.

---

## Phase 6: User Story 4 — Beaches & Culture migrated (P2)

**Independent test**: characterization suites 002 and 003 + architecture tests.

- [x] T047 [P] [US4] Beaches: unit tests for the `Beach` entity + `GetBeaches` interactor
- [x] T048 [P] [US4] Culture: unit tests for `Tradition` / `Dish` + `GetTraditions` / `GetDishes`
- [x] T049 [P] [US4] Beaches: Domain / Application / Infrastructure / Api restructure (same pattern as T030–T035); `CachedGetBeaches` TTL 24 h
- [x] T050 [P] [US4] Culture: same restructure; `CachedGetTraditions`, `CachedGetDishes`; confirm EF6 is removed (CUL-D3)
- [x] T051 [US4] Remove AutoMapper from `Directory.Packages.props` and the `AddMappingProfile` extension
- [x] T052 [US4] Delete the Beaches and Culture lines from `architecture-baseline.txt` (only the platform line remains until T059/T061)

---

## Phase 7: User Story 5 — Gateway routes correct (P2)

**Independent test**: quickstart §5.

- [x] T053 [US5] Table-driven gateway test in `tests/DiscoverCostaRica.AppHost.Tests/GatewayRoutesTests.cs` from contracts § Gateway, including the unknown path → 404 — table-driven over `GatewayRoutes` (no Aspire host needed) plus `GatewayUpstreamTests` proving every forwarded path is served
- [x] T054 [US5] Rewrite the YARP config in `DiscoverCostaRica.AppHost/AppHost.cs` with explicit `PathPattern` transforms + legacy aliases; remove the catch-all (BCH-D1, VOL-D7, PLT-D1)

---

## Phase 8: User Story 6 — Data ownership (P3) ⏸ deferred

> Deferred on purpose: moving production tables between schemas needs a maintenance window and a
> backup (quickstart §6), and changes no observable behaviour. The code is ready for it: each service
> owns its own `DbContext` and migrations, and nothing reads another service's tables.

**Independent test**: quickstart §6.

- [ ] T055 [P] [US6] Migration tests (Testcontainers): apply up and down per service, and check that row counts are preserved
- [ ] T056 [US6] Per-service migrations `MoveToOwnSchema`: `ALTER SCHEMA {svc} TRANSFER` + rename (`Geo.Province` → `geo.Province`), `HasDefaultSchema`
- [ ] T057 [US6] AppHost: one database user / connection string per service; document the grants in `docs/adr/0006-data-ownership.md`

---

## Phase 9: User Story 7 — Operable in production (P3)

- [x] T058 [P] [US7] Health checks: `/alive` + `/health` (SQL, Redis) mapped in all environments in `ServiceDefaults/Extensions.cs` (PLT-D6)
- [x] T059 [P] [US7] Remove the MongoDB logger (`Shared/Logging/*`, `LoggerFunction`, `IMongoClient` registration, the `mongodb` connection in AppHost); OTel logs only
- [x] T060 [P] [US7] `RequireHttpsMetadata = !IsDevelopment()` (PLT-D10); restrict the service discovery schemes to https in production

---

**Status (2026-10-14)**: Phases 1–7, 9 and most of 10 are done. US6 (schemas), T064 and T065
tasks remain (T064 needs Azure access for `azd infra gen`). 207 tests pass; coverage is about 98% line and 90% branch against an 80% CI gate.

## Phase 10: Polish & cross-cutting

- [x] T061 Delete `DiscoverCostaRica.Shared` (it should be empty now) and update `DiscoverCostaRica.sln`
- [x] T062 [P] Decisions recorded in `research.md` (R1–R10); `docs/architecture.md` marked superseded. (Originally: ADRs in `docs/adr/`: 0001 Clean Architecture, 0002 Result + presenter (R1), 0003 HybridCache (R4), 0004 batch resolve (R5), 0005 BuildingBlocks split (R6), 0006 data ownership (R9); mark the old ADR-02 / ADR-07 in `docs/architecture.md` as superseded.)
- [x] T063 [P] Update `README.md` + `docs/architecture.md` diagrams (C4 container view in Mermaid)
- [ ] T064 [P] `azd infra gen` and commit `infra/` (Bicep)
- [ ] T065 Run `/speckit-analyze`, then check SC-001..SC-006 and record the results in `specs/006-clean-architecture-migration/quickstart.md`

---

## Dependencies & Execution Order

```text
Phase 1 ─► Phase 2 ─► US1 (P3) ─► US2 Geo (P4) ─► US3 Volcano (P5)
                              └─► US4 Beaches/Culture (P6)  [parallel with US2/US3]
                              └─► US5 Gateway (P7)           [after US3 for volcano routes]
US2..US4 ─► US6 Schemas (P8) ─► US7 Ops (P9) ─► Polish (P10)
```

- US3 needs US2's `locations:resolve` endpoint and `Contracts.Geo`.
- US4 is independent of US2 and US3. One developer can take Beaches and another Culture.
- Within a story: tests → domain → application → infrastructure → api → shrink the architecture baseline.

## Parallel examples

```text
After Phase 2:  T026, T027, T028 (Geo tests)  ‖  T047, T048 (Beaches/Culture tests)
Phase 6:        T049 (Beaches)  ‖  T050 (Culture)
Phase 9:        T058  ‖  T059  ‖  T060
```

## Implementation strategy

1. **MVP = Phase 1 + Phase 2 + US1**: the contract is pinned and the architecture is guarded.
   This can ship on its own.
2. **Increment 2 = US2 + US3**: the broken headline feature works and scales.
3. **Increment 3 = US4 + US5**: uniform codebase and a correct public surface.
4. **Increment 4 = US6 + US7 + Polish**: production-grade ownership and operations.
