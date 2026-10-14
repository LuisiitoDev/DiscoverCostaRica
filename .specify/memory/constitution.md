<!--
Sync Impact Report
- Version change: (template) → 1.0.0
- Ratified: initial adoption, principles derived from Robert C. Martin's
  "Clean Architecture" (2017), "Clean Code" (2008) and the SOLID / component principles.
- Added sections: Core Principles (I–IX), Technology Constraints, Development Workflow, Governance
- Templates requiring updates:
  ✅ .specify/templates/plan-template.md (Constitution Check reads gates from this file, no edit needed)
  ✅ .specify/templates/spec-template.md (no principle-specific placeholders)
  ✅ .specify/templates/tasks-template.md (architecture tests are mandated by Principle VII)
- Follow-up TODOs: none
-->

# Discover Costa Rica Constitution

Discover Costa Rica is a set of read-mostly tourism microservices (Geo, Beaches, Culture,
Volcano) behind a YARP gateway. This constitution sets the non-negotiable engineering
rules. It is built on Robert C. Martin's Clean Architecture: **policy must never depend on
detail**.

## Core Principles

### I. The Dependency Rule (NON-NEGOTIABLE)

Source-code dependencies MUST point inward only:

```
Frameworks & Drivers  →  Interface Adapters  →  Use Cases  →  Entities
(EF Core, Redis,         (endpoints, presenters,  (interactors,   (domain model,
 ASP.NET, Refit, Entra)   gateways, mappers)       ports, models)  invariants)
```

- An inner circle MUST NOT name anything declared in an outer circle: no type, function,
  attribute or package reference.
- `Domain` projects MUST have zero package references beyond the BCL.
- `Application` projects MUST NOT reference ASP.NET Core, EF Core, StackExchange.Redis,
  Refit, MongoDB, Azure SDKs or `Microsoft.AspNetCore.Http` (that includes `StatusCodes`).
- Data that crosses a boundary MUST use simple structures owned by the inner circle
  (request/response models). Entities and EF rows MUST NOT leak outward as HTTP payloads.
- Every rule above MUST be enforced by automated architecture tests (Principle VII).

*Rationale*: Business rules outlive the framework. Inward-only dependencies let us swap
SQL, the cache, or the web host without touching the policy.

### II. Entities Encapsulate Enterprise Business Rules

- Entities are plain C# objects with behaviour, not anemic property bags. They protect
  their invariants with constructors or factory methods (`Create(...)`) that reject invalid
  state, private setters, and value objects where the identity is composite (for example
  `CantonCode(ProvinceId, CantonId)`).
- Entities MUST be persistence-ignorant: no EF attributes, no required navigation
  properties added only for the ORM, no `DbSet` knowledge.
- A rule that would hold even without this software (for example "a canton belongs to
  exactly one province", "a district code is unique only inside its canton") MUST live in
  the Domain.

### III. Use Cases Are the Application (Screaming Architecture)

- Each application behaviour is one **interactor** class named after the intent
  (`GetProvinces`, `GetVolcanoById`), not a catch-all `XService`. The folder tree MUST say
  "tourism catalog", not "MVC".
- Interactors receive a **Request model** and return a **Response model** wrapped in an
  application `Result<T>` with typed `Error`s (`NotFound`, `Validation`, `Unavailable`).
  They MUST NOT return transport concepts (HTTP status codes, `IResult`).
- **Ports** (gateway interfaces such as `IProvinceReader`, `IGeoLocationGateway`,
  `ICatalogCache`) are declared in the Use Case layer, which owns them, and are implemented
  outward. Repository interfaces MUST NOT live in Domain or Infrastructure.
- Cross-cutting application policy (caching, logging, validation) is added as decorators
  around interactors, never mixed into interactor bodies.

### IV. Interface Adapters Translate, They Never Decide

- **Controllers** (Minimal API endpoint handlers) only parse input into a Request model,
  call one interactor, and hand the outcome to a **Presenter**.
- **Presenters** map `Result<T>` to HTTP (status codes, the v1 response envelope,
  ProblemDetails). This is the only place HTTP semantics live.
- **Gateways** (EF repositories, the Geo HTTP client, cache adapters) implement ports and
  map between persistence or wire shapes and the inner models.
- Adapters MUST stay thin enough to be Humble Objects: no branching business logic.

### V. Frameworks Are Details; Main Is a Plugin

- EF Core, Redis, Refit, MongoDB, Entra ID and ASP.NET Core are details. They are
  reachable only through ports and are registered in the composition root.
- Each `*.Api` project is the **Main component**: the dirtiest module, and the only place
  that knows every concrete type. Wiring code MUST NOT leak into inner projects.
- A framework may be used in an inner circle only if it is a stable, policy-neutral
  abstraction from the BCL or `Microsoft.Extensions.*.Abstractions`. Any exception needs an
  ADR.

### VI. Service Autonomy and Explicit Contracts

- Each microservice owns its data (its own schema, migrations and connection identity).
  No service MAY read another service's tables.
- Synchronous calls between services go through a published **contract package**
  (`DiscoverCostaRica.Contracts.<Service>`): the request/response DTOs plus the typed client
  interface. Consumers depend on the contract, never on the provider's internals.
- Contracts are versioned. A breaking change needs a new API version, and the previous
  version stays served until every consumer has migrated.
- The public HTTP contract of v1 is frozen (see `specs/006-clean-architecture-migration/contracts`).

### VII. Test-First and Architecture Fitness

- New behaviour follows Red → Green → Refactor. Interactors MUST have unit tests that use
  in-memory fakes for ports. No mocks of types the project does not own.
- Every service MUST have architecture tests (NetArchTest or ArchUnitNET) that fail the
  build when Principle I is broken.
- Every gateway MUST have integration tests against real dependencies (Testcontainers for
  SQL Server and Redis). Every contract MUST have consumer tests.
- CI MUST block merge on build, tests, analyzers and architecture tests. Deploy jobs depend
  on those jobs.

### VIII. Observability and Operability

- Telemetry uses OpenTelemetry only (traces, metrics, structured logs) and exports via
  OTLP. Custom sinks such as direct MongoDB loggers are prohibited unless an ADR approves
  them.
- Each service exposes liveness and readiness health checks. Readiness covers its own
  dependencies.
- Every external call has a timeout and a resilience policy, and every cached value has a
  TTL.

### IX. Simplicity, SOLID and Component Cohesion

- Follow SRP, OCP, LSP, ISP and DIP at class level, and REP, CCP and CRP (cohesion) plus
  ADP, SDP and SAP (coupling) at component level. No dependency cycles between projects.
- YAGNI: no abstraction without a second implementation or a test seam that needs it.
  Every deviation goes in the plan's Complexity Tracking table.
- Shared libraries are split by reason to change. A single "Shared" assembly that mixes
  web, cache, logging and domain types is prohibited.
- Names are honest and spelled correctly. Comments explain *why*, not *what*.

## Technology Constraints

- Runtime: .NET 10 (LTS), C# latest. Nullable reference types and warnings-as-errors are on
  everywhere through `Directory.Build.props`.
- Package versions are pinned centrally in `Directory.Packages.props`. Pre-release
  packages are forbidden on `main`.
- Hosting: .NET Aspire AppHost for local orchestration; Azure Container Apps via `azd`
  for deployment, with IaC (Bicep) committed under `infra/`.
- Edge: YARP gateway. Identity: Microsoft Entra ID (OAuth 2.0 client credentials and app
  roles). Cache: Redis via `HybridCache`. Storage: Azure SQL, one schema per service.
- Mapping is explicit (hand-written or source-generated with Mapperly). Runtime
  reflection mappers (AutoMapper) are not used in new code.

## Development Workflow (Spec-Driven Development)

1. **Specify** (`/speckit-specify`): the *what* and *why*, technology-agnostic.
2. **Clarify** (`/speckit-clarify`): resolve every `[NEEDS CLARIFICATION]` before planning.
3. **Plan** (`/speckit-plan`): the *how*. The plan MUST pass the Constitution Check below.
4. **Tasks** (`/speckit-tasks`): ordered and testable, grouped by user story.
5. **Analyze** (`/speckit-analyze`): consistency gate across spec, plan and tasks.
6. **Implement** (`/speckit-implement`): test-first, one task per commit where practical.

Constitution Check gates (every plan answers each with PASS or a justified deviation):

- G1 Dependency Rule respected and enforced by architecture tests?
- G2 Domain persistence-ignorant, with invariants in entities?
- G3 One interactor per use case; ports owned by the Application layer?
- G4 HTTP concerns confined to presenters and endpoints?
- G5 Service owns its data and talks to others only through contracts?
- G6 Tests first: unit, integration (Testcontainers), contract and architecture?
- G7 OTel-only telemetry, TTL on cache, resilience on I/O?
- G8 No new complexity without a Complexity Tracking entry?

## Governance

- This constitution overrides any other practice or document in the repository. Where
  `docs/architecture.md` disagrees, this file wins until that document is updated.
- Amendments go through a PR that edits this file, bumps the version (SemVer: MAJOR =
  principle removed or redefined, MINOR = principle or section added, PATCH = wording) and
  updates the Sync Impact Report.
- Every PR review checks compliance. Deviations are recorded as ADRs in `docs/adr/`.

**Version**: 1.0.0 | **Ratified**: 2026-10-14 | **Last Amended**: 2026-10-14
