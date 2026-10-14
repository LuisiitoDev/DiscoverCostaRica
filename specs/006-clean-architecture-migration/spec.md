# Feature Specification: Clean Architecture Migration

**Feature Branch**: `006-clean-architecture-migration`

**Created**: 2026-10-14

**Status**: Draft (ready for `/speckit-plan` review)

**Input**: "Adapt Discover Costa Rica to Clean Architecture, following Uncle Bob's
recommendations, without breaking existing consumers, and fix the defects found while
reverse engineering specs 001–005."

## Context

> **Update 2026-10-14 (after phase 2)**: the characterization tests withdrew GEO-D1 and VOL-D3
> (false positives, see specs 001 and 004) and found PLT-D13 (spec 005).

Specs 001–005 describe the system **as built**. This feature moves it to the architecture
required by the constitution (v1.0.0) and fixes the catalogued defects. It is a **refactor
under a frozen contract**: v1 consumers see no change except the explicitly listed defect
fixes, which turn broken behaviour into the documented one.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - The architecture defends itself (Priority: P1) 🎯 MVP

An architect or reviewer can rely on the build to reject any change that breaks the
Dependency Rule, so the clean structure does not decay.

**Why this priority**: Without an automated fitness function, every other story erodes over
time. It is also the cheapest story to deliver, and it makes progress measurable.

**Independent Test**: Add a reference from a Domain or Application project to EF Core or
ASP.NET and check that the architecture test suite fails with a message naming the
offending type.

**Acceptance Scenarios**:

1. **Given** the architecture tests, **When** a Domain type references any non-BCL assembly,
   **Then** the build fails.
2. **Given** the tests, **When** an Application type references ASP.NET Core, EF Core, Redis,
   Refit, MongoDB or Azure SDK types, **Then** the build fails.
3. **Given** the tests, **When** a service references another service's projects (other
   than a `Contracts.*` package), **Then** the build fails.
4. **Given** CI, **When** a PR is opened, **Then** build, unit, architecture and
   characterization tests run and gate the merge, and deploy waits on them.

---

### User Story 2 - Geo catalog is correct and clean (Priority: P1)

A consumer gets correct provinces, cantons and districts. A developer finds one use case
per behaviour, with no framework types in the core.

**Why this priority**: Geo is the dependency of Volcano, and GEO-D2/D3 return wrong data.

**Independent Test**: Run the Geo characterization suite (v1 contract) plus the new
hierarchical routes against Geo alone with fakes, then against SQL in Testcontainers.

**Acceptance Scenarios**:

1. **Given** province 1 exists, **When** `GET /api/v1/geo/provinces/1` is called, **Then**
   200 with the province (regression guard; GEO-D1 was withdrawn).
2. **Given** canton 1 exists in provinces 1 and 2, **When** the districts of province 1 /
   canton 1 are requested through the hierarchical route, **Then** only districts of
   province 1 are returned (fixes GEO-D2 and GEO-D3).
3. **Given** the deprecated route `GET /api/v1/geo/districts/{cantonId}` is called with
   `?provinceId=1`, **Then** the result is scoped to that province. Without it, the as-built
   behaviour is kept, and a `Deprecation` header plus a `Link` to the successor route are
   added.
4. **Given** the cached province list, **When** its TTL (default 24 h) passes, **Then** the
   next request reloads it from storage (fixes GEO-D5).
5. **Given** any v1 Geo route, **Then** the response body matches the frozen v1 envelope
   byte for byte (property names, casing, null omission).

---

### User Story 3 - Volcano enrichment is efficient and resilient (Priority: P1)

A visitor gets every volcano with its location, quickly, even while Geo is slow or down.

**Why this priority**: Detail and by-province routes are broken today (VOL-D1), and the list does not
scale (VOL-D4).

**Independent Test**: With a fake Geo contract server, list 50 volcanoes. Check exactly
1 Geo call, then stop the fake and check that the list is still served from the last known
locations.

**Acceptance Scenarios**:

1. **Given** N volcanoes, **When** the list is requested uncached, **Then** Volcano makes
   exactly one batch call to Geo (`POST /api/v1/geo/locations:resolve`).
2. **Given** `GET /api/v1/volcanoes/5`, **Then** volcano 5 is returned with its location, and
   no query string is needed (fixes VOL-D1 and VOL-D5).
3. **Given** `GET /api/v1/volcanoes/province/2`, **Then** the volcanoes of province 2 come back
   with their locations (fixes VOL-D1 and VOL-D5).
4. **Given** `POST` / `DELETE` on any volcano route, **Then** 405 (fixes VOL-D2).
5. **Given** Geo is unavailable and locations were resolved before, **Then** volcanoes are
   served with the stale locations. **Given** they were never resolved, **Then**
   volcanoes are served without `location` and a warning is logged. The endpoint does not
   return 500.

---

### User Story 4 - Beaches and Culture follow the same architecture (Priority: P2)

A developer moving between services finds the same structure everywhere.

**Independent Test**: The characterization suites of 002 and 003 pass, and the architecture
tests pass for both services.

**Acceptance Scenarios**:

1. **Given** the migrated services, **Then** each behaviour is a single interactor, and
   caching is a decorator with a TTL (fixes BCH-D2 and CUL-D2).
2. **Given** the v1 routes, **Then** the responses match the frozen contract.
3. **Given** the Culture infrastructure project, **Then** it no longer references EF6
   (fixes CUL-D3).

---

### User Story 5 - Public gateway routes are correct (Priority: P2)

An API consumer uses short public paths, and every documented path works.

**Independent Test**: The gateway route table test calls every public path through the
AppHost with fake services.

**Acceptance Scenarios**:

1. `/beaches` → Beaches list; `/cantons/{p}` → Geo cantons; `/volcanoes`, `/volcanoes/{id}`,
   `/volcanoes/province/{p}` → Volcano (fixes BCH-D1, VOL-D7, PLT-D1).
2. Legacy public aliases `/canton/...` and `/volcano/...` keep working through explicit
   path rewrites.
3. An unknown path returns 404 from the gateway. It is no longer forwarded to Volcano.

---

### User Story 6 - Each service owns its data (Priority: P3)

An operator can grant each service access only to its own schema.

**Acceptance Scenarios**:

1. **Given** the migrations, **Then** the tables live in real SQL schemas (`geo`, `beach`,
   `culture`, `volcano`), and the data is preserved (tables are moved, not recreated).
2. **Given** a per-service connection identity, **When** Volcano tries to read `geo.*`,
   **Then** the database denies it.

---

### User Story 7 - Operable in production (Priority: P3)

An operator uses health probes and standard telemetry.

**Acceptance Scenarios**:

1. `/health/live` and `/health/ready` are served in every environment, and readiness checks
   SQL and Redis (fixes PLT-D6).
2. Errors go through OpenTelemetry logs. The MongoDB logger and its dependency are removed.
3. JWT metadata requires HTTPS outside Development (fixes PLT-D10).

---

### Edge Cases

- Cache stampede on a cold start: only one loader per key runs at a time.
- Batch resolve with unknown codes: those items come back `null` for that level. The batch
  never fails as a whole.
- Batch size limit: at most 200 items per request. More gives 400 with a validation message.
- Contract drift between Volcano and Geo: consumer tests in Volcano run against Geo's
  published contract DTOs.
- Partial migration: services migrate one at a time, and the architecture tests apply per
  service as each one migrates (the architecture baseline shrinks to empty).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Each service MUST be structured as Entities → Use Cases → Interface Adapters →
  Frameworks, with dependencies pointing inward only (Constitution I).
- **FR-002**: Each behaviour from specs 001–004 MUST be exactly one interactor with its own
  Request and Response models.
- **FR-003**: Interactors MUST return an application `Result<T>` with typed errors and no
  HTTP types. A presenter MUST translate it to the v1 envelope.
- **FR-004**: The v1 public contract (routes, envelope, messages, status codes) MUST stay
  backward compatible. Changes are limited to the defect fixes listed in this spec and to
  additive routes.
- **FR-005**: Geo MUST provide hierarchical, province-scoped canton and district routes, and
  a batch location-resolve operation for internal consumers.
- **FR-006**: Volcano MUST resolve locations with one batch call per uncached request and MUST
  serve stale locations when Geo is unavailable.
- **FR-007**: Every cached entry MUST have a TTL (default 24 h, configurable per key), and
  cache loading MUST be stampede-protected.
- **FR-008**: The `Shared` library MUST be replaced by components split by reason to change:
  application building blocks (pure), web building blocks, infrastructure building blocks,
  and per-service contract packages.
- **FR-009**: Architecture, unit, characterization, integration and contract tests MUST run in
  CI and gate both merge and deploy.
- **FR-010**: Package versions MUST be centrally managed, with no pre-release packages and no
  unused packages (EF6, Dapr leftovers, MongoDB).
- **FR-011**: Gateway routes MUST be explicit, with no catch-all forwarding.
- **FR-012**: Each service MUST own a dedicated SQL schema, and data MUST be preserved during
  the move.
- **FR-013**: Health endpoints MUST be available in all environments without leaking
  internals.

### Key Entities

- **Province / Canton / District** (Geo domain): entities with value-object identities
  `ProvinceCode`, `CantonCode(Province, Canton)`, `DistrictCode(Province, Canton, District)`.
- **Volcano**: entity referencing a `LocationCode` (province, canton, optional district).
- **Beach, Tradition, Dish**: entities with validated names and descriptions.
- **Result / Error**: application outcome type. `Error` has a code (`NotFound`, `Validation`,
  `Unavailable`, `Unexpected`) and a message.
- **ResolvedLocation** (Geo contract): province, canton and optional district names for one
  `LocationCode`.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 0 Dependency Rule violations reported by the architecture tests across all
  services.
- **SC-002**: 100% of v1 characterization tests pass unchanged after the migration, except
  the ones rewritten for the listed defect fixes.
- **SC-003**: The volcano list makes ≤ 1 Geo call for any N, and the uncached p95 is under
  300 ms for N = 50 in the integration environment.
- **SC-004**: Interactors have ≥ 90% line coverage, and the whole solution has ≥ 70%.
- **SC-005**: Every documented public gateway path returns a non-404 for valid input, and
  unknown paths return 404.
- **SC-006**: A new catalog service can be scaffolded by copying one service and passes the
  architecture tests with no exceptions added.

## Assumptions

- **A1**: Detail and by-province volcano responses gain `location`, matching the list. The
  response model already marked it required, so this counts as a defect fix (VOL-D5), not a
  breaking change.
- **A2**: The v1 envelope `{statusCode, message?, value?}` is kept for v1. A v2 with plain
  payloads and RFC 9457 ProblemDetails is a separate future feature.
- **A3**: Mapping is hand-written (small models), so AutoMapper is removed with no
  replacement library.
- **A4**: Services migrate one at a time, in this order: Geo → Volcano → Beaches → Culture.
- **A5**: The schema move (US6) runs in a maintenance window. Data size is small (reference
  data).
