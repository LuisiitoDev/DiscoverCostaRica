# Quickstart: Verify the Clean Architecture Migration

## Prerequisites

- .NET 10 SDK (pinned in `global.json`) and Docker (for Testcontainers)
- Aspire workload / CLI for end-to-end runs

## 1. Fast feedback (no Azure needed)

```bash
dotnet build -warnaserror
dotnet test tests/DiscoverCostaRica.ArchitectureTests      # US1: Dependency Rule
dotnet test --filter "Category=Unit"                       # interactors and entities
dotnet test tests/DiscoverCostaRica.CharacterizationTests  # frozen v1 contract
```

## 2. Prove the architecture defends itself (US1)

Temporarily add `using Microsoft.EntityFrameworkCore;` and a `DbSet` field to any
`*.Application` class and run the architecture tests. The tests must fail and name the type.
Revert the change.

## 3. Geo correctness (US2)

```bash
dotnet test tests/DiscoverCostaRica.IntegrationTests --filter "FullyQualifiedName~Geo"
```

These tests check: province by id returns 200; districts of (province 1, canton 1) contain
no rows from other provinces; the province cache expires after its TTL (fake time provider).

## 4. Volcano enrichment (US3)

```bash
dotnet test tests/DiscoverCostaRica.IntegrationTests --filter "FullyQualifiedName~Volcano"
```

These tests check: a WireMock Geo stub receives exactly one `locations:resolve` call for 50
volcanoes; after the stub stops, the list still returns 200 with stale locations; `POST
/api/v1/volcanoes/` returns 405.

## 5. Gateway (US5)

```bash
dotnet test tests/DiscoverCostaRica.AppHost.Tests --filter "Category=Gateway"
```

The tests are table-driven over `contracts/public-api-v1.md` § Gateway. `/does-not-exist`
returns 404.

## 6. Schema move (US6) — staging first

1. Back up the database.
2. `dotnet ef database update -p src/DiscoverCostaRica.Geo/DiscoverCostaRica.Geo.Infrastructure`
   (repeat per service).
3. Check `SELECT s.name, t.name FROM sys.tables t JOIN sys.schemas s ON s.schema_id = t.schema_id;`:
   only the `geo`, `beach`, `culture` and `volcano` schemas should be listed.
4. Roll back with `dotnet ef database update <PreviousMigration>` if needed.

## 7. End to end (Aspire)

```bash
dotnet run --project DiscoverCostaRica.AppHost
```

Open the Aspire dashboard. Call `GET {gateway}/volcanoes` and check that one trace spans
gateway → volcano → geo (`locations:resolve`).
