# Specifications (Spec-Driven Development)

This repository uses [GitHub Spec Kit](https://github.com/github/spec-kit). Specs are the
source of truth: code follows specs, not the other way round.

- **Constitution**: [`.specify/memory/constitution.md`](../.specify/memory/constitution.md)
  holds the non-negotiable rules, based on Uncle Bob's Clean Architecture.
- **Commands** (Claude Code skills in `.claude/skills/`): `/speckit-specify` →
  `/speckit-clarify` → `/speckit-plan` → `/speckit-tasks` → `/speckit-analyze` → `/speckit-implement`.

## Index

| # | Spec | Kind | Status |
|---|---|---|---|
| 001 | [Geo catalog](001-geo-catalog/spec.md) | Baseline (reverse-engineered) | As-built |
| 002 | [Beaches catalog](002-beaches-catalog/spec.md) | Baseline | As-built |
| 003 | [Culture catalog](003-culture-catalog/spec.md) | Baseline | As-built |
| 004 | [Volcano catalog](004-volcano-catalog/spec.md) | Baseline | As-built |
| 005 | [Platform cross-cutting](005-platform-cross-cutting/spec.md) | Baseline | As-built |
| 006 | [Clean Architecture migration](006-clean-architecture-migration/spec.md) | Feature | Draft: spec, plan, research, data model, contracts, tasks |

## How the baselines were produced (reverse engineering)

1. Read every endpoint, handler, service, repository, EF configuration, the gateway routes
   and the DI and auth wiring.
2. Restated the observed behaviour as user stories and acceptance scenarios, with the exact
   messages, routes and status codes the code produces today.
3. Recorded every gap between intent and behaviour as a numbered **defect**
   (`GEO-D2`, `VOL-D1`, …) instead of silently "fixing" it in the spec.
4. Froze the observable HTTP surface as the v1 contract
   ([006/contracts/public-api-v1.md](006-clean-architecture-migration/contracts/public-api-v1.md)).
   Characterization tests then pin it before any refactor.
5. Confirmed every suspected defect with a characterization test. Reading code is not
   proof: GEO-D1 and VOL-D3 looked like bugs and were withdrawn once the tests ran.

Baseline specs change only when the as-built behaviour changes. New behaviour always starts
with a new numbered spec.

## Branch note

Spec Kit's scripts expect feature branches named `NNN-feature`. If you work on another
branch, set `SPECIFY_FEATURE=006-clean-architecture-migration` before running the commands.
