# Verification Log

Date: 2026-06-08 local context.

## Inspected

| Area | Evidence |
| --- | --- |
| Inventory | `rg --files` returned API, Application, Domain, Infrastructure, Web, tests, CI, docs. |
| Git state | `git status --short -- .` returned `?? ./`. |
| .NET metadata | `PeblClone.sln:1-58`, `Directory.Build.props:1-10`, `Directory.Packages.props:1-10`, `global.json:1-6`. |
| API | `src/PeblClone.Api/Program.cs:5-43`, `src/PeblClone.Api/Controllers/WorkforceController.cs:6-40`, `src/PeblClone.Api/Extensions/SecurityHeadersExtensions.cs:7-15`. |
| Application | `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:5-120`, `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:6-67`, `src/PeblClone.Application/Workforce/IWorkforceReadModel.cs:5-12`. |
| Domain | `src/PeblClone.Domain/Workforce/Worker.cs:3-23`, `src/PeblClone.Domain/Workforce/Money.cs:3-15`, `src/PeblClone.Domain/Workforce/CountryProfile.cs:3-13`. |
| Infrastructure | `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:6-172`, `src/PeblClone.Infrastructure/DependencyInjection.cs:9-12`. |
| React | `src/PeblClone.Web/src/App.tsx:9-233`, `src/PeblClone.Web/src/services/workforceApi.ts:4-77`, `src/PeblClone.Web/src/features/QuoteBuilder.tsx:10-111`, `src/PeblClone.Web/src/services/workforceTypes.ts:1-97`. |
| Tests | `tests/PeblClone.Application.Tests/HiringQuoteCalculatorTests.cs:11-37`, `tests/PeblClone.Application.Tests/WorkforceDashboardServiceTests.cs:10-22`. |
| CI | `.github/workflows/ci.yml:1-33`. |

## Commands Run

| Command | Result |
| --- | --- |
| `rg --files` | Passed |
| `git status --short -- .` | Passed; repo is untracked under parent Git root |
| JSON parse for appsettings/package/tsconfig/launchSettings | Passed |
| XML parse for csproj/props | Passed |
| `npm install --package-lock-only --ignore-scripts` | Passed; no `node_modules`; npm reported 0 vulnerabilities |
| `dotnet --info` | Failed; command not recognized |

## Not Found

| Item | Evidence |
| --- | --- |
| EF Core | No `DbContext`, no migrations, no EF package in `Directory.Packages.props:1-10`. |
| Auth | No auth services in `src/PeblClone.Api/Program.cs:7-27`; no `[Authorize]` in `src/PeblClone.Api/Controllers/WorkforceController.cs:6-40`. |
| Background workers | No `IHostedService`/`BackgroundService` in `rg --files`/search. |
| Observability | Health checks at `src/PeblClone.Api/Program.cs:27,40`; no `ILogger` usage found. |
| API/UI tests | Only application tests found in `tests/PeblClone.Application.Tests/*.cs`. |
| CONTRIBUTING/SECURITY/LICENSE | Not present in inventory. |
| Docker/compose | Not present in inventory. |

## Uncertainties

- Build correctness cannot be fully verified until .NET SDK is installed.
- Frontend build was not run because dependencies were not installed; only lockfile generation and JSON parsing were verified.
- CI references .NET 10 and Node 22, but workflow was not executed locally.

