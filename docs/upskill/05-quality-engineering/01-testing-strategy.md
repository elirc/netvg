# 01 Testing Strategy

## Current Test Pyramid

| Layer | Present? | Evidence | What belongs here |
| --- | --- | --- | --- |
| Domain unit | Partial | `Worker` invariant exists at `src/PeblClone.Domain/Workforce/Worker.cs:15-22`; no direct domain tests found. | Pure invariants like attention and money currency. |
| Application unit | Yes | `tests/PeblClone.Application.Tests/HiringQuoteCalculatorTests.cs:11-37` and `tests/PeblClone.Application.Tests/WorkforceDashboardServiceTests.cs:10-22` | Business rules and DTO composition. |
| API integration | No | Controller exists at `src/PeblClone.Api/Controllers/WorkforceController.cs:12-40`; no WebApplicationFactory found. | Routing, status codes, ProblemDetails, auth. |
| Persistence integration | No | In-memory adapter at `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:6-172`; no EF Core found. | Queries, migrations, transactions, indexes. |
| UI component/E2E | No | UI exists at `src/PeblClone.Web/src/App.tsx:9-233`; no frontend test files found. | Loading, error, form, render states. |
| Contract | No | Manual DTO/TS mirror at `src/PeblClone.Application/Workforce/DashboardDtos.cs:5-68` and `src/PeblClone.Web/src/services/workforceTypes.ts:7-97`. | API schema compatibility. |

## What Not To Test

- Do not test that C# records store constructor arguments; e.g., `CountryProfile` at `src/PeblClone.Domain/Workforce/CountryProfile.cs:3-13`.
- Do not snapshot the entire dashboard HTML from `src/PeblClone.Web/src/App.tsx:50-232`; test behavior and key states.
- Do not test seed data as product truth except where tests intentionally document fixture assumptions, like total workers at `tests/PeblClone.Application.Tests/WorkforceDashboardServiceTests.cs:17`.

## Flake Prevention

- Inject time before testing date-sensitive behavior; current seed uses `DateTime.UtcNow` at `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:8`.
- Avoid tests that depend on current day offsets from seeded payroll at `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:156-162`.
- Keep application tests deterministic by constructing controlled read models instead of relying only on `InMemoryWorkforceReadModel`.

