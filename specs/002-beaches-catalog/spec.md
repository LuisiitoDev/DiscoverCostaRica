# Feature Specification: Beaches Catalog

**Feature Branch**: `002-beaches-catalog` (baseline, reverse-engineered)

**Created**: 2026-10-14

**Status**: Baseline (as-built) — describes current behaviour of `src/DiscoverCostaRica.Beaches`

**Input**: Reverse engineering of the existing Beaches API: "Let visitors discover the
beaches of Costa Rica."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - List all beaches (Priority: P1)

A visitor opens the beaches section and sees every beach with its name and description.

**Why this priority**: It is the only capability the service offers today.

**Independent Test**: Call the beach list with a `Beaches.Read` token and check the items.

**Acceptance Scenarios**:

1. **Given** beaches exist, **When** an authorized client requests the list, **Then** every
   beach (id, name, description) is returned with success.
2. **Given** no beaches exist, **Then** "not found" with the message "No beaches found.
   Please check later for updates."
3. **Given** the list was served once, **When** it is requested again, **Then** it comes
   from the distributed cache (key `Beach.Beaches`, no expiry as built).

---

### Edge Cases

- No token gives 401. Missing `Beaches.Read` app role gives 403.
- Plain HTTP requests are redirected to HTTPS (this service enables HTTPS redirection;
  Geo and Volcano do not).
- The beach is not linked to a location yet, so you cannot filter beaches by province.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST list all beaches with id, name and description.
- **FR-002**: Operation MUST require the `Beaches.Read` app role.
- **FR-003**: An empty catalog MUST be reported as "not found".
- **FR-004**: The list MUST be cached in the distributed cache.

### As-built HTTP surface (v1)

| Operation | Route | Cached |
|---|---|---|
| List beaches | `GET /api/v1/beaches/` | yes |

The OpenAPI metadata declares 200 / 400 / 404 / 500 and names the operation `GetBeaches`.

### Known defects found during reverse engineering

- **BCH-D1 (high)**: The gateway route `/beaches/{**}` gets the prefix `/api/v1/beaches`, so
  `/beaches` is forwarded to `/api/v1/beaches/beaches`. That path does not exist, so the
  public route returns 404.
- **BCH-D2 (medium)**: The cached list never expires.

### Key Entities

- **Beach**: Id (generated), name (required, ≤ 1000 chars), description (required,
  ≤ 10000 chars).

## Success Criteria *(mandatory)*

- **SC-001**: The public gateway route returns the beach list (fixes BCH-D1).
- **SC-002**: A cached list is answered in under 50 ms at p95.

## Assumptions

- Data is loaded out of band. The catalog is read-only.
- Linking beaches to Geo (province, canton) is a future feature and out of scope for the
  baseline.
