# 01 System Map

## Project Graph

| Project | Owns | Depends on | Anchors |
| --- | --- | --- | --- |
| API | HTTP routing, middleware, health, OpenAPI | Application, Infrastructure | `src/PeblClone.Api/PeblClone.Api.csproj:1-7`, `src/PeblClone.Api/Program.cs:20-41` |
| Application | Use cases, DTOs, ports | Domain | `src/PeblClone.Application/PeblClone.Application.csproj:1-6`, `src/PeblClone.Application/Workforce/IWorkforceReadModel.cs:5-12` |
| Domain | Business nouns and invariants | None | `src/PeblClone.Domain/PeblClone.Domain.csproj:1`, `src/PeblClone.Domain/Workforce/Worker.cs:3-23` |
| Infrastructure | Adapter implementations and seed data | Application, Domain | `src/PeblClone.Infrastructure/PeblClone.Infrastructure.csproj:1-7`, `src/PeblClone.Infrastructure/DependencyInjection.cs:9-12` |
| Web | React UI and typed client contracts | Browser, API contract | `src/PeblClone.Web/package.json:12-24`, `src/PeblClone.Web/src/services/workforceTypes.ts:7-97` |
| Tests | Application-level unit tests | Application, Infrastructure | `tests/PeblClone.Application.Tests/PeblClone.Application.Tests.csproj:1-12`, `tests/PeblClone.Application.Tests/HiringQuoteCalculatorTests.cs:11-37` |

## Public vs Private

Public API surface:

- `GET /api/workforce/dashboard` is declared at `src/PeblClone.Api/Controllers/WorkforceController.cs:12-18`.
- `POST /api/workforce/quotes` is declared at `src/PeblClone.Api/Controllers/WorkforceController.cs:20-40`.
- Health is mapped at `src/PeblClone.Api/Program.cs:40`.
- Development OpenAPI is mapped at `src/PeblClone.Api/Program.cs:33-36`.

Private implementation:

- Dashboard composition is private to the Application layer at `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:7-51`.
- Mapping helpers are private static methods at `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:54-119`.
- Seeded country, worker, and payroll data is private static state at `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:8-162`.

## Missing Subsystems

| Subsystem | Evidence | How to treat it |
| --- | --- | --- |
| EF Core persistence | No `DbContext`, no `Migrations/`, and Infrastructure is in-memory at `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:6-172`. | Investigate/future design, not existing behavior. |
| Authentication/authorization | No `[Authorize]`; controller has only `[ApiController]` and `[Route]` at `src/PeblClone.Api/Controllers/WorkforceController.cs:6-7`. | Possible risk for real production. |
| Background workers | No `IHostedService`/`BackgroundService`; async methods return seeded data at `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:164-171`. | Teach async flow, not queue semantics. |
| Observability | Health checks exist at `src/PeblClone.Api/Program.cs:27,40`; no `ILogger` usage found. | Risk: hard to diagnose production failures. |

## Verification Notes

- Inspected solution, project files, API, Application, Domain, Infrastructure, Web, tests, CI.
- `git status --short -- .` returned `?? ./`, meaning this repo is entirely untracked in the surrounding Git root.

