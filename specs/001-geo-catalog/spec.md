# Feature Specification: Geo Catalog (Administrative Divisions)

**Feature Branch**: `001-geo-catalog` (baseline, reverse-engineered)

**Created**: 2026-10-14

**Status**: Baseline (as-built) — describes current behaviour of `src/DiscoverCostaRica.Geo`

**Input**: Reverse engineering of the existing Geo API: "Expose Costa Rica's provinces,
cantons and districts so that visitors and other services can resolve where a place is."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Browse provinces (Priority: P1)

A client application lists the seven provinces of Costa Rica so a visitor can start
exploring by region.

**Why this priority**: Every other catalog (volcanoes, beaches) is located by province. The
platform cannot place anything without it.

**Independent Test**: Call the province list with a token that has the `Geo.Read` role and
check that all provinces come back with id and name.

**Acceptance Scenarios**:

1. **Given** provinces exist, **When** an authorized client requests the province list,
   **Then** the system returns every province (id, name) with success.
2. **Given** no provinces are stored, **When** the list is requested, **Then** the system
   answers "not found" with the message "No provinces found. Please check later for updates."
3. **Given** the list was served once, **When** it is requested again, **Then** it is
   served from the distributed cache without a database round trip.

---

### User Story 2 - Drill down province → canton → district (Priority: P1)

A visitor picks a province, sees its cantons, picks a canton and sees its districts.

**Why this priority**: This is the core navigation of the location hierarchy.

**Independent Test**: Request the cantons of province 1, then the districts of one of those
cantons, and check that every returned item belongs to its parent.

**Acceptance Scenarios**:

1. **Given** province P has cantons, **When** the cantons of P are requested, **Then** only
   cantons whose province is P are returned.
2. **Given** province P has no cantons, **When** they are requested, **Then** "not found"
   with the message "No cantons found. Please check later for updates."
3. **Given** canton C has districts, **When** the districts of C are requested, **Then** the
   districts of C are returned.
4. **Given** canton C has no districts, **Then** "not found" with the message "No districts
   found. Please check later for updates."

---

### User Story 3 - Resolve a single division by code (Priority: P2)

Another service (Volcano) knows a province, canton or district code and needs its name.

**Why this priority**: Used for enrichment, not for primary navigation.

**Independent Test**: Request province 1, canton (1,1) and district (1,1) individually.

**Acceptance Scenarios**:

1. **Given** province P exists, **When** it is requested by id, **Then** its id and name are
   returned. If it does not exist: "not found" ("No provice was found for {id}" — sic).
2. **Given** canton (P, C) exists, **When** it is requested, **Then** it is returned.
   Otherwise "not found" ("No canton was found for {cantonId}").
3. **Given** district (C, D) exists, **When** it is requested, **Then** it is returned.
   Otherwise "not found" ("No district was found for {districtId}").

---

### Edge Cases

- A canton code is unique **only inside its province**, and a district code only inside its
  canton (the storage keys are composite). See defects GEO-D2 and GEO-D3.
- Non-numeric ids fail route matching (404 from routing, not from the use case).
- A caller without a token gets 401. A caller with a token but without the `Geo.Read` app
  role gets 403.
- Unexpected failures return 500 with a generic ProblemDetails body.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST list all provinces.
- **FR-002**: System MUST return a province by its id.
- **FR-003**: System MUST list the cantons of a given province.
- **FR-004**: System MUST return a canton identified by (province id, canton id).
- **FR-005**: System MUST list the districts of a given canton.
- **FR-006**: System MUST return a district identified by (canton id, district id).
- **FR-007**: Every operation MUST require an authenticated caller holding the `Geo.Read`
  app role.
- **FR-008**: An empty result MUST be reported as "not found" with a human-readable message,
  never as an empty success.
- **FR-009**: The province list MUST be cached in the distributed cache (key
  `Geo.Provinces`). As built, the entry has no expiry.
- **FR-010**: The service MUST expose these operations under API version 1.

### As-built HTTP surface (v1)

| Operation | Route | Cached |
|---|---|---|
| List provinces | `GET /api/v1/geo/provinces` | yes |
| Province by id | `GET /api/v1/geo/provinces/{provinceId}` | no |
| Cantons of province | `GET /api/v1/geo/cantons/{provinceId}` | no |
| Canton by id | `GET /api/v1/geo/cantons/{provinceId}/{cantonId}` | no |
| Districts of canton | `GET /api/v1/geo/districts/{cantonId}` | no |
| District by id | `GET /api/v1/geo/districts/{cantonId}/{districtId}` | no |

Response body: the v1 envelope `{ "statusCode": int, "message"?: string, "value"?: T }`
(nulls are omitted). Full contract: `specs/006-clean-architecture-migration/contracts/`.

### Known defects found during reverse engineering

- **GEO-D1 (critical)**: "Province by id" always fails with 500. The repository passes the
  cancellation token as part of the primary key (`FindAsync([provinceId, cancellationToken])`).
  It also breaks Volcano's location enrichment (see 004).
- **GEO-D2 (high)**: "Districts of canton" filters by canton id only. Canton ids repeat
  across provinces, so districts from several provinces are mixed together.
- **GEO-D3 (high)**: "District by id" has the same ambiguity and can return a district
  from the wrong province.
- **GEO-D4 (low)**: The message typo "provice".
- **GEO-D5 (medium)**: The cached province list never expires.

### Key Entities

- **Province**: Top-level division. Id (1–7, assigned by the state, not generated) and name.
- **Canton**: Second level. Identified by (province id, canton id). Has a name and belongs
  to exactly one province.
- **District**: Third level. Identified by (province id, canton id, district id). Has a name
  and belongs to exactly one canton.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A cached province list is answered in under 50 ms at p95.
- **SC-002**: Every drill-down step returns only children of the requested parent (0 cross-
  province leaks).
- **SC-003**: Single-division lookups succeed for 100% of existing codes.

## Assumptions

- Reference data is loaded out of band. There is no seed or write API, and the catalog is
  read-only.
- Division codes follow the official INEC / Registro Nacional numbering.
- The GEO-D2 and GEO-D3 fixes need the province id on district queries. That is planned in
  spec 006.
