# Research & Decisions: Clean Architecture Migration

Each decision records: **Decision**, **Rationale** and **Alternatives considered**.

## R1. How strictly to apply Uncle Bob's ports (output port + presenter)

- **Decision**: Input boundary = `IUseCase<TRequest, TResponse>`. Interactors **return**
  `Result<TResponse>`. A presenter in the API adapter maps that result to HTTP.
- **Rationale**: In *Clean Architecture* (ch. 22) the Dependency Rule governs the direction of
  *source-code* dependencies, not the flow of control. Returning a response model the use
  case owns keeps the dependency inward, and it avoids per-use-case output-port interfaces
  that would only call `Results.Ok`. Presenters remain Humble Objects and are testable.
- **Alternatives**: (a) A strict output port (`IGetProvincesOutputPort` implemented by a
  presenter) adds about 2 interfaces per use case and buys nothing for request/response HTTP.
  (b) MediatR has had a commercial licence since v13 and hides the use case behind a bus,
  which works against "screaming architecture".

## R2. Where repository interfaces live

- **Decision**: In **Application** (`Ports/`), named after what the use case needs
  (`IGeoReadRepository`, `IVolcanoReadRepository`, `IGeoLocationGateway`).
- **Rationale**: Uncle Bob places gateway interfaces in the use-case circle. The entity
  circle knows nothing about storage. Today they sit in Domain, which is a common but
  incorrect "DDD-lite" placement.
- **Alternatives**: Keep them in Domain. Rejected: it couples enterprise rules to storage
  needs.

## R3. Entities vs EF Core rows

- **Decision**: Domain entities are persistence-ignorant classes with private setters and
  factories. EF maps them with Fluent configuration in Infrastructure (backing fields,
  `HasKey` on value-object components through owned or complex types). No separate row
  classes unless the mapping needs ORM-only properties.
- **Rationale**: EF Core 8+ complex types and field mapping let pure entities be persisted
  without duplicating every model (YAGNI, Constitution IX).
- **Alternatives**: Separate persistence models plus mappers. Kept as a fallback if a mapping
  gets in the way, and logged in Complexity Tracking if used.

## R4. Caching

- **Decision**: `Microsoft.Extensions.Caching.Hybrid` (`HybridCache`), with the L1 in-memory
  layer and L2 Redis. Caching decorators live in **Application** and depend only on the
  `HybridCache` abstract type from `Microsoft.Extensions.Caching.Abstractions`.
- **Rationale**: Stampede protection, TTL, tag invalidation and serialization come built in,
  which removes the hand-written `CacheService`. An abstraction package is allowed by
  Constitution V. What to cache and for how long is application policy.
- **Alternatives**: A custom `ICacheStore` port. Rejected because `HybridCache` already is
  that abstraction. Output caching at the edge: complementary, could be added later at the
  gateway.

## R5. Fixing N+1 in Volcano

- **Decision**: A new Geo operation `POST /api/v1/geo/locations:resolve` takes up to 200
  `LocationCode`s and returns `ResolvedLocation?[]` in the same order. Volcano calls it once
  per uncached request and caches resolved locations per code (TTL 24 h, stale-on-error).
- **Rationale**: 1 call instead of 3N. Geo stays the system of record (no data duplication).
  This matches SC-001 and SC-003.
- **Alternatives**: (a) A local read model replicated through events: best decoupling, but it
  needs a broker. Deferred to a future event-driven feature. (b) Preloading all of Geo
  (about 500 districts) into a Volcano memory cache: simple, but couples Volcano to Geo's
  full dataset.

## R6. Shared library split (component principles)

- **Decision**:

  | Component | Contents | Allowed deps |
  |---|---|---|
  | `BuildingBlocks.Application` | `Result<T>`, `Error`, `IUseCase<,>`, DI lifetime attributes | BCL only |
  | `BuildingBlocks.Web` | v1 envelope presenter, versioning, endpoint helpers, auth policies | ASP.NET Core |
  | `BuildingBlocks.Infrastructure` | Entra token provider, EF conventions, HybridCache wiring | EF Core, Azure.Identity, Redis |
  | `Contracts.Geo` | Geo DTOs + `LocationCode`, `ResolvedLocation` | BCL only |

- **Rationale**: CCP and CRP group what changes together, and SDP / SAP keep the stable
  abstract components at the bottom. Today `Shared` pulls ASP.NET, Redis, MongoDB and Refit
  into every Application layer (PLT-D3).
- **Alternatives**: One `SharedKernel`. Rejected because it mixes reasons to change.

## R7. Mapping

- **Decision**: Hand-written `static Response From(Entity)` methods in Application. AutoMapper
  is removed.
- **Rationale**: Fewer than 10 small models. Explicit code that is checked at compile time.
  No licence problem (AutoMapper ≥ 15 is commercial), and no outer layer that knows inner
  shapes (PLT-D12).
- **Alternatives**: Mapperly (source generated). Fine, but unnecessary at this size.

## R8. Testing strategy

- **Decision**:
  - **Characterization tests** (Feathers' "golden master") are written **first**, against the
    as-built v1 behaviour through `WebApplicationFactory` with in-memory fakes for ports.
    They pin the contract before any refactor.
  - **Unit**: interactors with hand-written fakes (xUnit v3).
  - **Architecture**: NetArchTest.Rules, one test class per service plus global rules.
  - **Integration**: Testcontainers (MsSql, Redis) for gateways and migrations.
  - **Contract**: Volcano consumer tests use `Contracts.Geo` DTOs against a WireMock.Net stub
    and a Geo provider test using the same JSON fixtures.
- **Rationale**: The refactor is safe only when the contract is pinned first.
  Testcontainers removes the need for Azure secrets in CI (PLT-D9).
- **Alternatives**: Pact (heavier tooling), Moq (not needed with fakes), FluentAssertions
  (commercial licence since v8). Plain `Assert` is used.

## R9. Data ownership

- **Decision**: Use EF `HasDefaultSchema("geo")` and similar per service. Migrations use
  `ALTER SCHEMA geo TRANSFER dbo.[Geo.Province]` followed by `sp_rename`. Each service gets
  its own contained DB user that has only its own schema.
- **Rationale**: This moves data in place with no copy, and it is reversible with a down
  migration.
- **Alternatives**: A separate database per service. Better isolation, but higher cost.
  Possible later, because the schema boundary makes it a connection-string change.

## R10. Gateway

- **Decision**: An explicit route table using `PathPattern` transforms, plus legacy aliases.
  The catch-all is removed. Later: rate limiting and output caching at the gateway.
- **Rationale**: Fixes BCH-D1, VOL-D7 and PLT-D1. Routes are tested with a table-driven test.
