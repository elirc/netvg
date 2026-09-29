# 05 Key Flows

## Flow: Dashboard Load

**Why it matters:** This is the main product experience. It crosses browser, API, application composition, and data adapter boundaries.

**Open first:** `src/PeblClone.Web/src/App.tsx:14-35` - effect-driven load.

| Step | Owner | File | What happens | Data shape | Risk |
| --- | --- | --- | --- | --- | --- |
| 1 | React | `src/PeblClone.Web/src/App.tsx:14-35` | `useEffect` calls `getDashboard`, guards unmounted state. | `WorkforceDashboard \| null` | No visible error state, only mock fallback. |
| 2 | Client service | `src/PeblClone.Web/src/services/workforceApi.ts:23-35` | Fetches `/api/workforce/dashboard`; returns mock on catch. | `{ data, source }` | Silent fallback can hide outages. |
| 3 | Controller | `src/PeblClone.Api/Controllers/WorkforceController.cs:12-18` | Delegates to application service. | `ActionResult<WorkforceDashboardDto>` | No auth boundary. |
| 4 | Application | `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:9-28` | Reads countries/workers/payroll and computes summary. | `WorkforceSummaryDto` | Reads are serial; future network calls may add latency. |
| 5 | Application | `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:30-51` | Maps collections for UI. | Lists of DTOs | In-memory sorting/paging does not scale forever. |
| 6 | UI | `src/PeblClone.Web/src/App.tsx:84-227` | Renders metrics, tables, alerts, countries, payroll, quote builder. | TS interfaces | Large tables have no virtualization/pagination. |

**Validation & authorization:** No `[Authorize]` or policy is present on `src/PeblClone.Api/Controllers/WorkforceController.cs:6-40`.  
**Persistence & side effects:** Data comes from `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:164-171`. No EF Core persistence found.  
**Tests:** Dashboard summary is covered at `tests/PeblClone.Application.Tests/WorkforceDashboardServiceTests.cs:10-22`; no API or UI test found.  
**Juniors miss:** A successful UI can be showing mock data from `src/PeblClone.Web/src/services/workforceApi.ts:29-34`.  
**Seniors notice:** The port boundary at `src/PeblClone.Application/Workforce/IWorkforceReadModel.cs:5-12` makes persistence migration possible.  
**Drill:** Add a trace note for how `workersNeedingAttention` reaches the metric card at `src/PeblClone.Web/src/App.tsx:100-103`.  
**Self-grade:** Basic = can name files. Solid = can explain DTO transformation. Strong = can name scaling/auth risks.

## Flow: Hiring Quote Success

**Why it matters:** This flow teaches validation, business rules, HTTP error contracts, and frontend async form state.

**Open first:** `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:12-65` - business rules.

| Step | Owner | File | What happens | Data shape | Risk |
| --- | --- | --- | --- | --- | --- |
| 1 | React form | `src/PeblClone.Web/src/features/QuoteBuilder.tsx:17-31` | Prevents default, sends country/type/compensation/currency. | `HiringQuoteRequest` | Only min input; business validation is server-side. |
| 2 | Client service | `src/PeblClone.Web/src/services/workforceApi.ts:37-42` | POSTs JSON to quote endpoint. | `HiringQuote` | Error handling falls into mock path. |
| 3 | Controller | `src/PeblClone.Api/Controllers/WorkforceController.cs:20-40` | Calls calculator and returns `BadRequest` on failure. | `ProblemDetails` or DTO | Model validation attributes are absent. |
| 4 | Application | `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:12-34` | Rejects non-positive comp, unsupported country/type. | `Result<HiringQuoteDto>` | Request currency is trusted. |
| 5 | Application | `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:36-65` | Applies rates/fees and returns quote. | Quote DTO | Rates are hard-coded. |
| 6 | UI | `src/PeblClone.Web/src/features/QuoteBuilder.tsx:80-105` | Renders total, fees, onboarding, requirements. | `HiringQuote` | No user-visible API failure message. |

**Validation & authorization:** Validation exists in application at `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:12-34`; no authorization boundary found.  
**Persistence & side effects:** Country lookup reads the port at `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:17-19`.  
**Tests:** Success and unsupported contractor country are covered at `tests/PeblClone.Application.Tests/HiringQuoteCalculatorTests.cs:11-37`.  
**Juniors miss:** Frontend mock quote duplicates backend fee logic at `src/PeblClone.Web/src/services/workforceApi.ts:50-75`.  
**Seniors notice:** Duplicated quote math creates contract drift risk.  
**Drill:** Write a failing test for `MonthlyCompensation <= 0` using `tests/PeblClone.Application.Tests/HiringQuoteCalculatorTests.cs:11-37` as pattern.  
**Self-grade:** Basic = can submit quote. Solid = can trace failure. Strong = can propose contract test or shared mock strategy.

## Flow: Worker Attention Derivation

**Why it matters:** It teaches a domain invariant: a worker requires attention if status says so or unresolved high-risk compliance exists.

| Step | Owner | File | What happens | Data shape | Risk |
| --- | --- | --- | --- | --- | --- |
| 1 | Domain | `src/PeblClone.Domain/Workforce/Worker.cs:20-22` | Computes `RequiresAttention`. | Boolean invariant | Depends on check risk semantics. |
| 2 | Seed data | `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:88-105` | Mateus has `NeedsAttention` and high/medium checks. | Worker | Seed may overfit tests. |
| 3 | Application | `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:23-24` | Counts attention workers and open compliance items. | Summary DTO | Summary and list must stay consistent. |
| 4 | Application | `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:32-36` | Sorts attention workers first. | Worker list | Sorting is implicit product behavior. |
| 5 | UI | `src/PeblClone.Web/src/App.tsx:100-103,142-148` | Shows metric, status, progress. | Rendered dashboard | Accessibility labels are minimal. |

**Tests:** Current dashboard test asserts one attention worker and first row attention at `tests/PeblClone.Application.Tests/WorkforceDashboardServiceTests.cs:17-21`.  
**Drill:** Change only seed data mentally: if Mateus status becomes Active but high check remains unresolved, should he still require attention? Explain using `src/PeblClone.Domain/Workforce/Worker.cs:20-22`.  
**Self-grade:** Basic = yes/no. Solid = explains invariant. Strong = proposes focused domain test.

## Flow: Country Readiness

**Why it matters:** Country data drives availability, compliance requirements, payroll timing, and quote feasibility.

| Step | Owner | File | What happens | Data shape | Risk |
| --- | --- | --- | --- | --- | --- |
| 1 | Domain | `src/PeblClone.Domain/Workforce/CountryProfile.cs:3-13` | Defines country contract. | Record | No score range invariant. |
| 2 | Infrastructure | `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:10-67` | Seeds five countries. | `IReadOnlyList<CountryProfile>` | Data is static, no source of truth. |
| 3 | Application | `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:37-41` | Sorts countries by active usage and score. | `CountryReadinessDto[]` | Sort rule is not tested directly. |
| 4 | UI | `src/PeblClone.Web/src/App.tsx:178-200` | Displays readiness cards and availability pills. | Country cards | No drilldown for required docs/benefits. |
| 5 | Quote | `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:26-34` | Rejects unsupported EOR/contractor types. | `Result` failure | Payroll-only availability is not checked. |

**Tests:** Japan contractor rejection is covered at `tests/PeblClone.Application.Tests/HiringQuoteCalculatorTests.cs:26-37`.  
**Drill:** Add a country score boundary test proposal. What should happen if `ComplianceScore` is 101?

## Flow: Payroll Approval Queue

**Why it matters:** Payroll is a high-risk business area: deadlines, money, approval state, and country context must be trustworthy.

| Step | Owner | File | What happens | Data shape | Risk |
| --- | --- | --- | --- | --- | --- |
| 1 | Domain | `src/PeblClone.Domain/Workforce/PayrollRun.cs:1-10` | Defines payroll run. | Record | No invariant for date order or amount currency. |
| 2 | Infrastructure | `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:156-162` | Seeds payroll runs. | List | Dates derive from `Today`; tests may shift. |
| 3 | Application | `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:47-50` | Sorts by approval deadline. | `PayrollRunDto[]` | No pagination or approval command. |
| 4 | UI | `src/PeblClone.Web/src/App.tsx:203-223` | Displays deadline, gross payroll, approved/pending. | Cards | Pending state is display-only. |

**Persistence & side effects:** No approval mutation or side effect exists.  
**Tests:** No direct payroll ordering test found.  
**Drill:** Write a test name only: `GetDashboardAsync_OrdersPayrollRunsByApprovalDeadline`.

## Flow: API Composition And Middleware

**Why it matters:** Runtime setup decides what every request passes through before it reaches code you wrote.

| Step | Owner | File | What happens | Data shape | Risk |
| --- | --- | --- | --- | --- | --- |
| 1 | Host | `src/PeblClone.Api/Program.cs:5` | Creates builder. | WebApplicationBuilder | Environment-driven config is implicit. |
| 2 | Services | `src/PeblClone.Api/Program.cs:7-27` | Registers CORS, app/infrastructure, controllers, OpenAPI, ProblemDetails, health. | DI container | No auth/rate limiting/logging registration. |
| 3 | Pipeline | `src/PeblClone.Api/Program.cs:31-41` | Adds headers, dev OpenAPI, HTTPS redirect, CORS, health, controllers. | Middleware chain | Header middleware runs before endpoints. |
| 4 | Headers | `src/PeblClone.Api/Extensions/SecurityHeadersExtensions.cs:7-15` | Adds basic security headers. | HTTP response | No CSP/HSTS here. |

**Tests:** No API integration tests found.  
**Drill:** Explain why middleware order matters for CORS and controllers using `src/PeblClone.Api/Program.cs:31-41`.

## Flow: CI Build

**Why it matters:** CI is the team contract that decides whether a contribution is safe enough to review.

| Step | Owner | File | What happens | Data shape | Risk |
| --- | --- | --- | --- | --- | --- |
| 1 | GitHub Actions | `.github/workflows/ci.yml:3-6` | Runs on push to main and PR. | Workflow event | No branch protection shown. |
| 2 | Backend job | `.github/workflows/ci.yml:9-18` | Setup .NET 10, restore, build, test. | Solution | Local SDK missing blocks local validation. |
| 3 | Frontend job | `.github/workflows/ci.yml:20-33` | Setup Node 22, `npm ci`, build. | Bundle | No lint step in CI despite script. |

**Tests:** CI references test project via solution, but local `dotnet` could not run.  
**Drill:** Add a proposed CI step for `npm run lint`, then explain what failure mode it catches.

## Flow: Mock Fallback

**Why it matters:** Fallbacks are useful for demos and dangerous in production if they hide outages or stale data.

| Step | Owner | File | What happens | Data shape | Risk |
| --- | --- | --- | --- | --- | --- |
| 1 | Client service | `src/PeblClone.Web/src/services/workforceApi.ts:23-35` | Dashboard catch returns mock. | `source: 'mock'` | Silent API failures. |
| 2 | UI | `src/PeblClone.Web/src/App.tsx:69-72` | Shows source badge. | Badge text | Users may ignore it. |
| 3 | Client service | `src/PeblClone.Web/src/services/workforceApi.ts:43-77` | Quote catch calculates fallback quote. | Quote DTO | Business logic duplicated. |
| 4 | Mock data | `src/PeblClone.Web/src/services/mockDashboard.ts:1-221` | Local dashboard mirror. | Hard-coded data | Contract drift from backend DTOs. |

**Tests:** No frontend tests found.  
**Drill:** Design a production flag that disables mock fallback; list files touched and tests needed.

