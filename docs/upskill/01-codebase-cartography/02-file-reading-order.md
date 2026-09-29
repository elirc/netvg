# 02 File Reading Order

## Junior Order

| Order | File | Look for |
| --- | --- | --- |
| 1 | `README.md:1-46` | What the app claims to do and how to run it. |
| 2 | `Directory.Build.props:1-10` | Repo-wide C# defaults. |
| 3 | `src/PeblClone.Api/Program.cs:5-43` | How services and endpoints enter the runtime. |
| 4 | `src/PeblClone.Api/Controllers/WorkforceController.cs:6-40` | The two API endpoints. |
| 5 | `src/PeblClone.Application/Workforce/DashboardDtos.cs:5-68` | Response shape returned to the UI. |
| 6 | `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:7-51` | How the dashboard is assembled. |
| 7 | `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:10-67` | Country seed data. |
| 8 | `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:69-154` | Worker seed data. |
| 9 | `src/PeblClone.Web/src/services/workforceTypes.ts:7-97` | TypeScript copy of API contracts. |
| 10 | `src/PeblClone.Web/src/App.tsx:84-227` | How dashboard data is displayed. |

## Mid-Level Order

| File | Look for |
| --- | --- |
| `src/PeblClone.Application/Workforce/IWorkforceReadModel.cs:5-12` | Port boundary for storage. |
| `src/PeblClone.Infrastructure/DependencyInjection.cs:9-12` | Adapter lifetime and binding. |
| `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:12-65` | Validation, branching, money math. |
| `src/PeblClone.Web/src/features/QuoteBuilder.tsx:17-31` | Form event flow and async state. |
| `src/PeblClone.Web/src/services/workforceApi.ts:37-77` | API call and fallback duplication risk. |
| `tests/PeblClone.Application.Tests/HiringQuoteCalculatorTests.cs:11-37` | Current business-rule test style. |
| `.github/workflows/ci.yml:8-33` | CI contract for backend and frontend. |

## Senior Order

| File | Question |
| --- | --- |
| `src/PeblClone.Api/Program.cs:7-18` | Is CORS scoped tightly enough for environments? |
| `src/PeblClone.Api/Extensions/SecurityHeadersExtensions.cs:7-15` | What headers are present and what is missing? |
| `src/PeblClone.Domain/Workforce/Money.cs:7-14` | Is currency safety strong enough for payroll-grade money? |
| `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:9-11` | What happens when these reads become network/database calls? |
| `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:8` | What risks come from computing dates from `DateTime.UtcNow`? |
| `src/PeblClone.Web/src/services/workforceApi.ts:29-34,43-77` | Should production silently fall back to mock data? |
| `src/PeblClone.Web/src/App.tsx:132-152` | What happens with hundreds or thousands of workers? |

