# Risk Register

| Risk | Evidence | Impact | Likelihood | Suggested test | Suggested fix | Confidence |
| --- | --- | --- | --- | --- | --- | --- |
| No authentication | `src/PeblClone.Api/Program.cs:7-27`; `src/PeblClone.Api/Controllers/WorkforceController.cs:6-40` | Sensitive data exposure if deployed | High if exposed | API 401/403 tests | Add authn/authz policies | High |
| Mock fallback hides outage | `src/PeblClone.Web/src/services/workforceApi.ts:29-34,43-77` | Users see stale/demo data | Medium | UI failed-fetch test | Env-gate fallback, show error | High |
| Quote math duplicated | `src/PeblClone.Web/src/services/workforceApi.ts:50-75`; `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:36-65` | Contract drift | High | Contract test | Remove duplicate policy | High |
| No persistence | `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:6-172` | No durable state | High for real product | Integration tests after EF | Add DB adapter behind port | High |
| Time-dependent seed data | `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:8` | Flaky date tests | Medium | Inject clock test | Add clock abstraction | High |
| No API tests | Controller at `src/PeblClone.Api/Controllers/WorkforceController.cs:12-40`; no WebApplicationFactory found | Route/status regressions | Medium | API smoke tests | Add integration test project | High |
| No frontend tests | UI at `src/PeblClone.Web/src/App.tsx:9-233`; no test files found | UI regressions | Medium | Component/E2E tests | Add Vitest/RTL or Playwright | High |
| Manual C# <-> TS contracts | `src/PeblClone.Application/Workforce/DashboardDtos.cs:5-68`; `src/PeblClone.Web/src/services/workforceTypes.ts:7-97` | Runtime mismatch | Medium | Schema/contract test | Generate client/types | High |
| Missing observability | Health only at `src/PeblClone.Api/Program.cs:27,40`; no `ILogger` usage found | Hard incident diagnosis | Medium | Log assertion/smoke | Add structured logs/metrics | High |
| No rate limiting | No rate limiter registration at `src/PeblClone.Api/Program.cs:7-27` | Abuse/cost risk | Medium if public | API load test | Add rate limiting | Medium |

