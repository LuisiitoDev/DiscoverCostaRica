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

- [ ] T001 Add `global.json` pinning the .NET 10 SDK (rollForward `latestFeature`) at repo root
- [ ] T002 Add `Directory.Build.props` (Nullable, TreatWarningsAsErrors, AnalysisLevel latest-recommended, ImplicitUsings, LangVersion latest) at repo root
- [ ] T003 Add `Directory.Packages.props` with Central Package Management; move every `PackageReference` version into it; unify EF Core 10.0.x, OpenApi 10.0.x (drop rc), AutoMapper removed later in T051
- [ ] T004 [P] Remove unused packages: `EntityFramework` 6.5.1 (all Infrastructure csproj), `Microsoft.AspNetCore.Http` 2.3.0, `System.Linq.Async`; delete `DiscoverCostaRica.AppHost/Constants/DaprCompoonents.cs`
- [ ] T005 [P] Add `.editorconfig` (file-scoped namespaces, `_camelCase` private fields, no BOM) and fix typos: `CachedProviceService`→`CachedProvinceService`, `ResourceParemeterBuilder`→`ResourceParameterBuilder`, `AddGlobalExeption`→`AddGlobalException`, rename `DiscoverCostaRica.CultureSerice.Api.http`
- [ ] T006 Add `.github/workflows/ci.yml`: restore → build `-warnaserror` → test (unit, architecture, characterization) → coverage report; make `azure-dev.yml` run only after `ci` succeeds on `main` (`workflow_run`)
- [ ] T007 [P] Add `.github/dependabot.yml` (nuget + github-actions, weekly)

**Checkpoint**: The solution builds with zero warnings, and CI runs on every PR.

---

## Phase 2: Foundational (blocks every story)

**Purpose**: Building blocks, contracts, test harnesses. ⚠️ No service migration until this is done.

- [ ] T008 Create `src/BuildingBlocks/DiscoverCostaRica.BuildingBlocks.Application` (BCL only): `Error`, `ErrorType`, `Result<T>`, `IUseCase<TRequest,TResponse>`; move DI lifetime attributes here from `Shared/Attributes`
- [ ] T009 Create `src/BuildingBlocks/DiscoverCostaRica.BuildingBlocks.Web`: `V1EnvelopePresenter` (`Result<T>` → `IResult` per data-model §1), `BuildEndpointGroup` versioning helper, `RoutesConstants`, auth policy attribute and constants (moved from `Shared`)
- [ ] T010 [P] Create `src/BuildingBlocks/DiscoverCostaRica.BuildingBlocks.Infrastructure`: Entra client-credentials token provider (singleton, cached credential), `AddCatalogDbContext<T>(schema)`, `AddCatalogHybridCache()` (Redis L2)
- [ ] T011 [P] Create `src/Contracts/DiscoverCostaRica.Contracts.Geo` (BCL only) with the DTOs from data-model §6
- [ ] T012 Update `DiscoverCostaRica.SourceGenerators` to recognise attributes from `BuildingBlocks.Application` and to emit `Decorate<IUseCase<,>, …>` for generic decorators
- [ ] T013 Create `tests/DiscoverCostaRica.ArchitectureTests` (NetArchTest.Rules): rules from spec US1 #1–#3, plus a per-service **allow-list** (initially Geo, Volcano, Beaches, Culture) so that the build stays green during the migration
- [ ] T014 Create `tests/DiscoverCostaRica.CharacterizationTests` with a `WebApplicationFactory<TProgram>` per service, a test auth handler (fake `roles` claims) and in-memory port fakes swapped in through `ConfigureTestServices`
- [ ] T015 [P] Characterization tests, Geo v1: every row in contracts/public-api-v1.md § Geo (as built). GEO-D1 is pinned as `Skip="GEO-D1"`
- [ ] T016 [P] Characterization tests, Beaches v1
- [ ] T017 [P] Characterization tests, Culture v1
- [ ] T018 [P] Characterization tests, Volcano v1 (VOL-D1/D2/D5 pinned as skipped)
- [ ] T019 Create `tests/DiscoverCostaRica.IntegrationTests` with `MsSqlFixture` and `RedisFixture` (Testcontainers) and a migration runner per service DbContext
- [ ] T020 [P] Unit test `V1EnvelopePresenter` in `tests/DiscoverCostaRica.BuildingBlocks.UnitTests` (JSON byte-equality with the as-built envelope)

**Checkpoint**: Contract pinned, fitness functions running, building blocks ready.

---

## Phase 3: User Story 1 — The architecture defends itself (P1) 🎯 MVP

**Goal**: Violations fail the build. **Independent test**: quickstart §2.

- [ ] T021 [US1] Add a global rule: no project references another service's projects except `Contracts.*`
- [ ] T022 [US1] Add a rule: `*.Application` must not depend on `Microsoft.AspNetCore.*`, `Microsoft.EntityFrameworkCore*`, `StackExchange.Redis`, `Refit`, `MongoDB.*`, `Azure.*`
- [ ] T023 [US1] Add a rule: `*.Domain` depends on `System.*` only
- [ ] T024 [US1] Add a rule: types under `Ports/` are interfaces, and their implementations live in `*.Infrastructure`
- [ ] T025 [US1] Document how to shrink the allow-list in `tests/DiscoverCostaRica.ArchitectureTests/README.md`

**Checkpoint**: US1 is delivered. Every following story removes one allow-list entry.

---

## Phase 4: User Story 2 — Geo correct and clean (P1)

**Goal**: Geo on Clean Architecture; GEO-D1..D5 fixed. **Independent test**: quickstart §3.

### Tests first

- [ ] T026 [P] [US2] Entity tests: `ProvinceCode` range, `CantonCode` / `DistrictCode` equality, blank names rejected, in `tests/DiscoverCostaRica.Geo.UnitTests/Domain/`
- [ ] T027 [P] [US2] Interactor tests with `FakeGeoReadRepository` for all 7 use cases (not found messages per contract), in `tests/DiscoverCostaRica.Geo.UnitTests/UseCases/`
- [ ] T028 [P] [US2] Integration tests: `GeoReadRepository` against MsSql; same canton id in two provinces must not leak (GEO-D2, D3); set-based `ResolveAsync`
- [ ] T029 [US2] Flip the Skip on the GEO-D1 characterization test; add tests for the hierarchical and deprecated routes (`Deprecation` + `Link` headers)

### Implementation

- [ ] T030 [US2] Domain: `Province`, `Canton`, `District` and the value objects in `src/DiscoverCostaRica.Geo/DiscoverCostaRica.Geo.Domain/` (remove the navigation-heavy `*Model` classes)
- [ ] T031 [US2] Application: `Ports/IGeoReadRepository.cs`, plus `UseCases/*` interactors with Request/Response models and static `From()` mappers; delete `GeoService`, `ProvinceService` and `Interfaces/`
- [ ] T032 [US2] Application: `Caching/CachedGetProvinces` decorator using `HybridCache` (key `geo:provinces`, TTL 24 h)
- [ ] T033 [US2] Infrastructure: rewrite `GeoReadRepository` (fixes `FindAsync` GEO-D1; scoped district queries); EF configurations map entities and value objects; move `IGeoContext` to an internal type
- [ ] T034 [US2] Api: `Endpoints/*` controllers → interactor → `V1EnvelopePresenter`; add the hierarchical routes and `POST locations:resolve` (validation 1..200); delete `Profiles/MappingProfile.cs` and `Handler/GeoHandler.cs`
- [ ] T035 [US2] Composition root `Program.cs`: register via generated DI, HybridCache, DbContext; remove the AutoMapper and Mongo registrations
- [ ] T036 [US2] Remove Geo from the architecture allow-list; all Geo tests green

**Checkpoint**: Geo fully migrated; v1 contract intact; GEO-D1..D5 closed.

---

## Phase 5: User Story 3 — Volcano efficient and resilient (P1)

**Goal**: 1 Geo call, stale-on-error, routes fixed. **Depends on**: Phase 4 (batch endpoint).
**Independent test**: quickstart §4.

### Tests first

- [ ] T037 [P] [US3] Interactor tests with `FakeVolcanoReadRepository` + `FakeGeoLocationGateway`: list, by id and by province all include location; codes deduplicated before the gateway call
- [ ] T038 [P] [US3] Contract tests: `GeoLocationGateway` against a WireMock.Net stub using JSON fixtures shared with the Geo provider test (`tests/fixtures/geo-resolve/*.json`)
- [ ] T039 [P] [US3] Integration tests: exactly 1 resolve call for 50 volcanoes; stub down → stale locations served; never resolved → no `location`, still 200
- [ ] T040 [US3] Flip VOL-D1/D2/D5 characterization skips; add a 405 test for POST/DELETE

### Implementation

- [ ] T041 [US3] Domain: `Volcano` entity + `LocationCode` VO; remove `VolcanoModel` and the mixed `VolcanoService.*` namespaces
- [ ] T042 [US3] Application: `Ports/IVolcanoReadRepository`, `Ports/IGeoLocationGateway`; interactors `GetVolcanoes`, `GetVolcanoById`, `GetVolcanoesByProvince`; delete `LocationService`, `MapperVolcanoService`, `Constants/ApiRoutes.cs`
- [ ] T043 [US3] Application: `CachedGetVolcanoes` decorator (TTL 1 h)
- [ ] T044 [US3] Infrastructure: `Geo/IGeoApi` (Refit, `Contracts.Geo` DTOs) + `GeoLocationGateway` (per-code HybridCache, stale-on-error, standard resilience handler); remove `IGeoDiscoverCostaRica` from Shared and its global registration in ServiceDefaults (PLT-D5)
- [ ] T045 [US3] Api: `MapGet` endpoints with path binding (`{id:int}`, `{provinceId:int}`) → presenter; delete `VolcanoHandler`, `MappingProfile`
- [ ] T046 [US3] Remove Volcano from the allow-list; all Volcano tests green

**Checkpoint**: VOL-D1..D6, D8 closed. SC-003 measured.

---

## Phase 6: User Story 4 — Beaches & Culture migrated (P2)

**Independent test**: characterization suites 002 and 003 + architecture tests.

- [ ] T047 [P] [US4] Beaches: unit tests for the `Beach` entity + `GetBeaches` interactor
- [ ] T048 [P] [US4] Culture: unit tests for `Tradition` / `Dish` + `GetTraditions` / `GetDishes`
- [ ] T049 [P] [US4] Beaches: Domain / Application / Infrastructure / Api restructure (same pattern as T030–T035); `CachedGetBeaches` TTL 24 h
- [ ] T050 [P] [US4] Culture: same restructure; `CachedGetTraditions`, `CachedGetDishes`; confirm EF6 is removed (CUL-D3)
- [ ] T051 [US4] Remove AutoMapper from `Directory.Packages.props` and the `AddMappingProfile` extension
- [ ] T052 [US4] Remove Beaches and Culture from the allow-list (the allow-list is now **empty**)

---

## Phase 7: User Story 5 — Gateway routes correct (P2)

**Independent test**: quickstart §5.

- [ ] T053 [US5] Table-driven gateway test in `tests/DiscoverCostaRica.AppHost.Tests/GatewayRoutesTests.cs` from contracts § Gateway, including the unknown path → 404
- [ ] T054 [US5] Rewrite the YARP config in `DiscoverCostaRica.AppHost/AppHost.cs` with explicit `PathPattern` transforms + legacy aliases; remove the catch-all (BCH-D1, VOL-D7, PLT-D1)

---

## Phase 8: User Story 6 — Data ownership (P3)

**Independent test**: quickstart §6.

- [ ] T055 [P] [US6] Migration tests (Testcontainers): apply up and down per service, and check that row counts are preserved
- [ ] T056 [US6] Per-service migrations `MoveToOwnSchema`: `ALTER SCHEMA {svc} TRANSFER` + rename (`Geo.Province` → `geo.Province`), `HasDefaultSchema`
- [ ] T057 [US6] AppHost: one database user / connection string per service; document the grants in `docs/adr/0006-data-ownership.md`

---

## Phase 9: User Story 7 — Operable in production (P3)

- [ ] T058 [P] [US7] Health checks: `/health/live` + `/health/ready` (SQL, Redis) mapped in all environments in `ServiceDefaults/Extensions.cs` (PLT-D6)
- [ ] T059 [P] [US7] Remove the MongoDB logger (`Shared/Logging/*`, `LoggerFunction`, `IMongoClient` registration, the `mongodb` connection in AppHost); OTel logs only
- [ ] T060 [P] [US7] `RequireHttpsMetadata = !IsDevelopment()` (PLT-D10); restrict the service discovery schemes to https in production

---

## Phase 10: Polish & cross-cutting

- [ ] T061 Delete `DiscoverCostaRica.Shared` (it should be empty now) and update `DiscoverCostaRica.sln`
- [ ] T062 [P] ADRs in `docs/adr/`: 0001 Clean Architecture, 0002 Result + presenter (R1), 0003 HybridCache (R4), 0004 batch resolve (R5), 0005 BuildingBlocks split (R6), 0006 data ownership (R9); mark the old ADR-02 / ADR-07 in `docs/architecture.md` as superseded
- [ ] T063 [P] Update `README.md` + `docs/architecture.md` diagrams (C4 container view in Mermaid)
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
- Within a story: tests → domain → application → infrastructure → api → shrink the allow-list.

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
