# Feature Specification: Volcano Catalog with Location Enrichment

**Feature Branch**: `004-volcano-catalog` (baseline, reverse-engineered)

**Created**: 2026-10-14

**Status**: Baseline (as-built) — describes current behaviour of `src/DiscoverCostaRica.Volcano`

**Input**: Reverse engineering of the existing Volcano API: "Let visitors discover Costa
Rica's volcanoes and know exactly where each one is."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - List volcanoes with their location (Priority: P1)

A visitor lists all volcanoes. Each one shows its province, canton and district names.

**Why this priority**: This is the headline feature and the only cross-service use case.

**Independent Test**: Call the volcano list with a `Volcano.Read` token and check that each
item has a `location` with province and canton (district only when known).

**Acceptance Scenarios**:

1. **Given** volcanoes exist, **When** the list is requested, **Then** each volcano (id,
   name, description) is returned with `location { province, canton, district? }` taken
   from the Geo service.
2. **Given** a volcano has no district code, **Then** `location.district` is absent.
3. **Given** no volcanoes exist, **Then** "not found" ("Volcanos were not found").
4. **Given** a previous request, **Then** later requests come from the cache (key
   `Volcano.Volcanos`), including the enriched locations.

---

### User Story 2 - Get one volcano (Priority: P2)

A visitor opens the detail page of a volcano.

**Independent Test**: Request volcano by id.

**Acceptance Scenarios**:

1. **Given** volcano V exists, **When** it is requested by id, **Then** id, name and
   description are returned (as built: **without** location).
2. **Given** it does not exist, **Then** "not found" ("Volcano was not found").

---

### User Story 3 - Volcanoes in a province (Priority: P2)

A visitor who browses a province sees its volcanoes.

**Acceptance Scenarios**:

1. **Given** province P has volcanoes, **When** they are requested, **Then** they are
   returned (as built: **without** location).
2. **Given** none exist, **Then** "not found" ("Volcanos were not found").

---

### Edge Cases

- Geo unavailable or returning errors: as built, the Refit client throws, and the
  standard resilience handler retries and then fails, so the list returns 500.
- Enrichment cost: as built, each volcano triggers 3 HTTP calls to Geo (province, canton,
  district), all in parallel. N volcanoes means up to 3N calls (N+1 pattern).
- The service authenticates to Geo with an Entra client-credentials token attached by a
  delegating handler.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST list all volcanoes enriched with location names.
- **FR-002**: System MUST return a volcano by id.
- **FR-003**: System MUST list the volcanoes of a province.
- **FR-004**: Every operation MUST require the `Volcano.Read` app role.
- **FR-005**: Location data MUST come from the Geo service. Volcano stores only codes.
- **FR-006**: The enriched list MUST be cached.

### As-built HTTP surface (v1)

| Operation | Route | Cached |
|---|---|---|
| List volcanoes | `ANY /api/v1/volcanoes/` | yes |
| Volcano by id | `ANY /api/v1/volcanoes/{id}` | no |
| By province | `ANY /api/v1/volcanoes/province/{provinceId}` | no |

### Known defects found during reverse engineering

- **VOL-D1 (critical, confirmed)**: The handlers bind `id` from the **query string** (`[FromQuery] int id`),
  but the routes declare `{id}` / `{provinceId}` path parameters. `GET /api/v1/volcanoes/5`
  returns 400 unless `?id=5` is also sent. "By province" reads `?id=` and ignores the path.
- **VOL-D2 (high, confirmed)**: The endpoints use `Map` instead of `MapGet`, so they answer every HTTP
  verb (POST, DELETE, and so on).
- **VOL-D3 (withdrawn)**: Reverse engineering expected the list to fail because of GEO-D1.
  GEO-D1 turned out to be a false positive, and the characterization test shows the list
  returns every volcano with its location.
- **VOL-D4 (high)**: N+1 fan-out to Geo (3 calls per volcano).
- **VOL-D5 (medium, confirmed)**: Detail and by-province responses leave out `location`, even though
  the response model marks it as required. The responses are inconsistent.
- **VOL-D6 (medium)**: The typed Geo client declares wrong return types for list
  operations (`Result<DtoProvince>` where lists are returned).
- **VOL-D7 (high)**: The gateway route `/volcano/{**}` becomes `/api/v1/volcanoes/volcano/...`,
  which does not exist. The gateway catch-all `/{**}` sends **every unknown path** to Volcano.
- **VOL-D8 (low)**: The cache never expires, so stale Geo names are kept forever.

### Key Entities

- **Volcano**: Id (assigned, not generated), name, description, province id and canton id
  (required), district id (optional).
- **Location** (read model): province { id, name }, canton { id, name }, district? { id,
  name }.

## Success Criteria *(mandatory)*

- **SC-001**: The volcano list costs at most **1** call to Geo, whatever N is (fixes VOL-D4).
- **SC-002**: The volcano list keeps working while Geo is down, served from the last known
  locations.
- **SC-003**: All three operations return the same volcano shape.

## Assumptions

- Volcano reference data is loaded out of band. Read-only.
- Geo is the system of record for division names.
