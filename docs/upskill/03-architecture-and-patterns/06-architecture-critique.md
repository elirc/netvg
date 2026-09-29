# 06 Architecture Critique

## Strong Choices

| Choice | Evidence | Why it is strong |
| --- | --- | --- |
| Layered project graph | `PeblClone.sln:5-14` | Separates API, Application, Domain, Infrastructure, and tests. |
| Application port for data | `src/PeblClone.Application/Workforce/IWorkforceReadModel.cs:5-12` | Allows persistence migration without changing controllers. |
| Thin controller | `src/PeblClone.Api/Controllers/WorkforceController.cs:12-40` | Keeps HTTP concerns separate from business rules. |
| Domain invariants | `src/PeblClone.Domain/Workforce/Worker.cs:15-22`, `src/PeblClone.Domain/Workforce/Money.cs:7-14` | Puts reusable rules near domain data. |
| CI exists | `.github/workflows/ci.yml:8-33` | Establishes contribution gate. |

## Risks And Tradeoffs

| Priority | Risk | Evidence | Confidence | Suggested move |
| --- | --- | --- | --- | --- |
| High | No auth boundary | `src/PeblClone.Api/Controllers/WorkforceController.cs:6-40`, `src/PeblClone.Api/Program.cs:7-27` | High | Add authentication/authorization before exposing real data. |
| High | Mock fallback can hide outage | `src/PeblClone.Web/src/services/workforceApi.ts:29-34,43-77` | High | Gate fallback by environment and show errors. |
| Medium | Pricing duplicated in UI fallback | `src/PeblClone.Web/src/services/workforceApi.ts:50-75`, `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:36-65` | High | Remove fallback quote math or generate mocks from fixture. |
| Medium | No persistence | `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:6-172` | High | Add EF/read model adapter when product needs state. |
| Medium | No API/UI tests | Existing tests only at `tests/PeblClone.Application.Tests/*.cs` | High | Add WebApplicationFactory and UI tests. |
| Low | No observability beyond health | `src/PeblClone.Api/Program.cs:27,40` | High | Add structured logs and metrics around flows. |

## If I Owned This For Three Months

1. Security first: auth, tenant scoping, and API integration tests. Anchors: current unauthenticated controller at `src/PeblClone.Api/Controllers/WorkforceController.cs:6-40`.
2. Contract reliability: generate TypeScript contracts or add schema tests between `src/PeblClone.Application/Workforce/DashboardDtos.cs:5-68` and `src/PeblClone.Web/src/services/workforceTypes.ts:7-97`.
3. Persistence migration: replace `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:6-172` with a scoped database adapter behind `src/PeblClone.Application/Workforce/IWorkforceReadModel.cs:5-12`.
4. Observability: add structured logs around dashboard and quote flows in `src/PeblClone.Api/Controllers/WorkforceController.cs:12-40`.
5. Frontend resilience: replace silent fallback at `src/PeblClone.Web/src/services/workforceApi.ts:29-34` with explicit error states.

## Migration Strategy Per Recommendation

| Recommendation | Migration | Tests | Rollback |
| --- | --- | --- | --- |
| Auth | Add auth middleware/policies, then protect endpoints. | API 401/403 tests. | Feature flag policy enforcement in non-prod first. |
| Persistence | Add EF adapter behind existing port. | Existing application tests plus DB integration tests. | Keep in-memory adapter selectable by config. |
| Contract generation | Add OpenAPI client generation. | Compile TS and API smoke test. | Keep manual types until generated client passes. |
| Error states | Add UI error state before disabling fallback. | Component/E2E test with failed API. | Re-enable demo fallback only in dev. |

## Verification Notes

Confirmed: layer structure, in-memory adapter, current tests, CI, security headers. Hypothesis: production exposure would require auth, persistence, and observability; those are not implemented in this repository.

