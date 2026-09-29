# 02 Writing Tests Here

Commands:

| Command | Status | Notes |
| --- | --- | --- |
| `dotnet test PeblClone.sln` | Inferred | CI uses this shape at `.github/workflows/ci.yml:16-18`; local SDK unavailable. |
| `dotnet test --filter HiringQuoteCalculatorTests` | Inferred | Test class is `tests/PeblClone.Application.Tests/HiringQuoteCalculatorTests.cs:9-38`. |
| `dotnet test --filter WorkforceDashboardServiceTests` | Inferred | Test class is `tests/PeblClone.Application.Tests/WorkforceDashboardServiceTests.cs:8-23`. |
| `cd src/PeblClone.Web; npm run build` | Inferred | Script at `src/PeblClone.Web/package.json:8`; CI runs it at `.github/workflows/ci.yml:32-33`. |

## Recipes

1. Happy quote path: follow `tests/PeblClone.Application.Tests/HiringQuoteCalculatorTests.cs:11-24`.
2. Validation failure: add negative compensation test against `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:12-15`.
3. Unsupported country: test failure branch at `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:21-24`.
4. Permission failure: not possible until auth exists; start from missing auth evidence at `src/PeblClone.Api/Controllers/WorkforceController.cs:6-40`.
5. Cross-tenant rejection: not possible until tenant IDs/policies exist; use future route design around `src/PeblClone.Api/Controllers/WorkforceController.cs:7`.
6. Async side effect: not present; future hosted worker should honor cancellation like controller tokens at `src/PeblClone.Api/Controllers/WorkforceController.cs:14,25`.
7. Query invalidation/cache: not present; current read model returns static data at `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:164-171`.
8. UI state: add a component/E2E test for loading at `src/PeblClone.Web/src/App.tsx:42-47` and mock badge at `src/PeblClone.Web/src/App.tsx:69-72`.

