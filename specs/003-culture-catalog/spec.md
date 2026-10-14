# Feature Specification: Culture Catalog (Traditions & Typical Dishes)

**Feature Branch**: `003-culture-catalog` (baseline, reverse-engineered)

**Created**: 2026-10-14

**Status**: Baseline (as-built) — describes current behaviour of `src/DiscoverCostaRica.Culture`

**Input**: Reverse engineering of the existing Culture API: "Show visitors Costa Rican
traditions and typical dishes."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Discover traditions (Priority: P1)

A visitor browses Costa Rican traditions with a description and an image.

**Why this priority**: Core cultural content of the platform.

**Independent Test**: Call the traditions list with a `Culture.Read` token.

**Acceptance Scenarios**:

1. **Given** traditions exist, **When** they are requested, **Then** every tradition (id,
   name, description, imageUrl) is returned.
2. **Given** none exist, **Then** "not found" with the message "No traditions found. Please
   check later for updates."
3. **Given** a previous request, **Then** later requests are served from the cache (key
   `Culture.Traditions`).

---

### User Story 2 - Discover typical dishes (Priority: P1)

A visitor browses typical dishes with ingredients and preparation.

**Why this priority**: Equal value to traditions. Both are independent.

**Independent Test**: Call the dish list with a `Culture.Read` token.

**Acceptance Scenarios**:

1. **Given** dishes exist, **Then** every dish (id, name, description, ingredients,
   preparation, imageUrl) is returned.
2. **Given** none exist, **Then** "not found" with the message "No dishes found. Please check
   later for updates."
3. **Given** a previous request, **Then** later requests are served from the cache (key
   `Culture.Dishes`).

---

### Edge Cases

- `imageUrl` is optional in storage. An absent value is serialized as an empty string or
  omitted.
- No token gives 401. Missing `Culture.Read` gives 403.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST list all traditions.
- **FR-002**: System MUST list all typical dishes.
- **FR-003**: Both operations MUST require the `Culture.Read` app role.
- **FR-004**: Empty lists MUST be reported as "not found".
- **FR-005**: Both lists MUST be cached independently.

### As-built HTTP surface (v1)

| Operation | Route | Cached |
|---|---|---|
| List traditions | `GET /api/v1/traditions/tradition` | yes |
| List dishes | `GET /api/v1/traditions/dish` | yes |

### Known defects found during reverse engineering

- **CUL-D1 (low)**: The route group is named `traditions` but also hosts dishes. A v2 should
  expose `/culture/traditions` and `/culture/dishes`.
- **CUL-D2 (medium)**: The cache never expires.
- **CUL-D3 (low)**: The infrastructure project references Entity Framework 6 even though it
  uses EF Core. The package is unused.

### Key Entities

- **Tradition**: Id, name (required), description (required), imageUrl (optional).
- **Dish**: Id, name (required), description, ingredients (required), preparation
  (required), imageUrl (optional).

## Success Criteria *(mandatory)*

- **SC-001**: Both lists are reachable through the gateway (`/tradition`, `/dish`).
- **SC-002**: A cached list is answered in under 50 ms at p95.

## Assumptions

- Read-only catalog. Content is curated out of band.
- Content is single-language (Spanish or English, not localized).
