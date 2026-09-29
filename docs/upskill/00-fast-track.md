# 00 Fast Track

## Weekend Win

Goal: understand one UI-to-API flow, one business rule, and one test without changing product code.

## Commands

| Command | Status | Evidence |
| --- | --- | --- |
| `dotnet restore PeblClone.sln` | Inferred, not run | Solution exists at `PeblClone.sln:1-58`; SDK pinned at `global.json:1-6`. |
| `dotnet test PeblClone.sln` | Inferred, not run | CI runs restore/build/test at `.github/workflows/ci.yml:16-18`; local `dotnet --info` failed. |
| `dotnet run --project src/PeblClone.Api` | Inferred, not run | Launch URL is `http://localhost:5088` at `src/PeblClone.Api/Properties/launchSettings.json:4-11`. |
| `cd src/PeblClone.Web; npm install; npm run dev` | Inferred, not run | Scripts are in `src/PeblClone.Web/package.json:6-10`; Vite port is `src/PeblClone.Web/vite.config.ts:6-9`. |
| `npm install --package-lock-only --ignore-scripts` | Verified pass | Lockfile was generated; no `node_modules` installed. |

## First 10 Files

1. `README.md:1-46` for product identity and run commands.
2. `Directory.Build.props:1-10` for C# defaults: `net10.0`, nullable references, warnings as errors.
3. `Directory.Packages.props:1-10` for central package versions.
4. `src/PeblClone.Api/Program.cs:5-43` for runtime composition.
5. `src/PeblClone.Api/Controllers/WorkforceController.cs:6-40` for public API routes.
6. `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:5-120` for dashboard use-case composition.
7. `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:6-67` for validation and quote rules.
8. `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:6-172` for the current data adapter.
9. `src/PeblClone.Web/src/services/workforceApi.ts:4-77` for browser-to-API calls and mock fallback.
10. `src/PeblClone.Web/src/App.tsx:9-233` for the main UI.

## Trace These Flows

- Dashboard load: `src/PeblClone.Web/src/App.tsx:14-35` -> `src/PeblClone.Web/src/services/workforceApi.ts:23-35` -> `src/PeblClone.Api/Controllers/WorkforceController.cs:12-18` -> `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:7-51`.
- Hiring quote: `src/PeblClone.Web/src/features/QuoteBuilder.tsx:17-31` -> `src/PeblClone.Web/src/services/workforceApi.ts:37-77` -> `src/PeblClone.Api/Controllers/WorkforceController.cs:20-40` -> `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:12-65`.

## Small Safe Change

Docs-only: add one more self-grading question to `docs/upskill/04-code-reading-gym/01-annotation-drills.md`. Product-code change later: add one application test around negative compensation, following `tests/PeblClone.Application.Tests/HiringQuoteCalculatorTests.cs:11-37`.

## Teach-Back

Explain why the API controller does not calculate quote totals itself. Anchor your answer to controller delegation at `src/PeblClone.Api/Controllers/WorkforceController.cs:27-39` and business rules at `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:12-65`.

## What This Skips

This skips persistence, authentication, hosted background work, observability, deployment, and frontend tests. Those are not implemented in the repo; treat them as design exercises, not existing behavior.

