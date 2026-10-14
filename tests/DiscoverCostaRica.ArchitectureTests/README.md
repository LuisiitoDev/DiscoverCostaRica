# Architecture tests

Fitness functions for the [constitution](../../.specify/memory/constitution.md) (Principle I, the
Dependency Rule) and spec 006 User Story 1. They run in CI with every other test, so a change
that points a dependency outward fails the build.

## Rules

| Rule id | What it enforces | Source |
|---|---|---|
| `domain-references-nothing` | `*.Domain` has no package or project references | .csproj |
| `application-allowed-references` | `*.Application` references only its own Domain, `BuildingBlocks.Application`, `Contracts.*`, `Microsoft.Extensions.*.Abstractions` and Scrutor | .csproj |
| `infrastructure-allowed-references` | `*.Infrastructure` points inward only (no Api, no Web building block) | .csproj |
| `no-cross-service-references` | A service reaches another only through `Contracts.*` | .csproj |
| `shared-components-independent-of-services` | BuildingBlocks, Contracts and ServiceDefaults never reference a service | .csproj |
| `pure-components-reference-nothing` | `BuildingBlocks.Application` and `Contracts.*` have no dependencies | .csproj |
| `no-legacy-shared` | Nothing references the legacy `DiscoverCostaRica.Shared` | .csproj |
| `application-no-framework-types` | Application types do not use ASP.NET Core, EF Core, Redis, Refit, MongoDB, Azure SDK or AutoMapper types | IL |
| `inner-circles-bcl-only` | Domain, `BuildingBlocks.Application` and `Contracts.*` types depend on `System.*` and themselves only | IL |
| `ports-are-interfaces` | Types in `*.Application.Ports` are interfaces | IL |
| `use-cases-in-application` | `IUseCase<,>` implementations live in an Application assembly | IL |

Project-graph rules read the `.csproj` files instead of compiled metadata, because the compiler
drops references that no code uses yet. That is how the unused `Culture.Application → Geo.Infrastructure`
edge was found.

## The baseline is a ratchet

`architecture-baseline.txt` lists the violations that existed when the rules were introduced,
grouped by the spec 006 task that removes them.

- A violation that is **not** listed fails the build. Fix the code; never add new lines.
- A listed violation that has **gone away** also fails the build, and the message tells you which
  line to delete. Every improvement is locked in.

When a service finishes its migration (for example Geo, T036), its lines disappear. The file
is empty when spec 006 is done.

## Try it

```bash
dotnet test tests/DiscoverCostaRica.ArchitectureTests
```

To see a rule fire, add `<PackageReference Include="Microsoft.EntityFrameworkCore" />` to any
`*.Domain.csproj`, or use `Microsoft.AspNetCore.Http.IResult` in an Application class, then run
the tests again.
