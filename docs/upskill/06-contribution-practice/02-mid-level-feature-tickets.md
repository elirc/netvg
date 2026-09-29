# 02 Mid-Level Feature Tickets

## Ticket 1: Add API Integration Tests
**Difficulty:** Medium **Time:** 1 day  
**Read first:** `src/PeblClone.Api/Controllers/WorkforceController.cs:12-40`; `.github/workflows/ci.yml:16-18`  
**Acceptance criteria:** - [ ] `GET /api/workforce/dashboard` returns 200. - [ ] invalid quote returns 400 ProblemDetails.

## Ticket 2: Add Auth Design And Stub Policy
**Difficulty:** Medium **Time:** 1-2 days  
**Read first:** `src/PeblClone.Api/Program.cs:7-27`; `src/PeblClone.Api/Controllers/WorkforceController.cs:6-40`  
**Acceptance criteria:** - [ ] RFC explains authn/authz. - [ ] Tests describe 401/403 behavior.

## Ticket 3: Replace Quote Mock Math
**Difficulty:** Medium **Time:** 1 day  
**Read first:** `src/PeblClone.Web/src/services/workforceApi.ts:43-77`; `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:36-65`  
**Acceptance criteria:** - [ ] No duplicated pricing policy in UI fallback. - [ ] Demo mode still works.

## Ticket 4: Add Generated Client Proof Of Concept
**Difficulty:** Hard **Time:** 2 days  
**Read first:** `src/PeblClone.Api/Program.cs:33-36`; `src/PeblClone.Web/src/services/workforceTypes.ts:7-97`  
**Acceptance criteria:** - [ ] Generated types compile. - [ ] Manual type drift risk documented.

## Ticket 5: Add EF Read Model Spike
**Difficulty:** Hard **Time:** 2-3 days  
**Read first:** `src/PeblClone.Application/Workforce/IWorkforceReadModel.cs:5-12`; `src/PeblClone.Infrastructure/DependencyInjection.cs:9-12`  
**Acceptance criteria:** - [ ] Scoped adapter. - [ ] Migration plan. - [ ] Application tests still pass.

## Ticket 6: Add Payroll Approval Command Design
**Difficulty:** Hard **Time:** 2 days  
**Read first:** `src/PeblClone.Domain/Workforce/PayrollRun.cs:1-10`; `src/PeblClone.Web/src/App.tsx:203-223`  
**Acceptance criteria:** - [ ] Defines idempotency, authorization, audit, rollback. - [ ] No mutation without tests.

## Ticket 7: Add Observability Around Quote Flow
**Difficulty:** Medium **Time:** 1 day  
**Read first:** `src/PeblClone.Api/Controllers/WorkforceController.cs:20-40`; `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:12-65`  
**Acceptance criteria:** - [ ] Structured logs avoid sensitive data. - [ ] Metrics count success/failure.

## Ticket 8: Add Frontend Error Boundary/State
**Difficulty:** Medium **Time:** 1 day  
**Read first:** `src/PeblClone.Web/src/App.tsx:42-47`; `src/PeblClone.Web/src/services/workforceApi.ts:23-35`  
**Acceptance criteria:** - [ ] API failure visible. - [ ] Demo fallback explicitly labeled.

## Ticket 9: Add Pagination Design
**Difficulty:** Medium **Time:** 1 day  
**Read first:** `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:32-36`; `src/PeblClone.Web/src/App.tsx:132-152`  
**Acceptance criteria:** - [ ] Defines server vs client ownership. - [ ] Includes performance test idea.

## Ticket 10: Add Country Detail Drawer
**Difficulty:** Medium **Time:** 1-2 days  
**Read first:** `src/PeblClone.Web/src/App.tsx:178-200`; `src/PeblClone.Application/Workforce/DashboardDtos.cs:34-43`  
**Acceptance criteria:** - [ ] Shows docs and benefits. - [ ] Uses existing DTO fields.

