# 04 Review Katas

## Kata 1: Move Quote Math Into Controller
**Author intent:** simplify quote endpoint.  
**Fake diff summary:** inline fee calculation in `CreateQuote`.  
**Resembles:** `src/PeblClone.Api/Controllers/WorkforceController.cs:27-39`  
**Expected findings:** Blocking: violates thin-controller boundary.  
**Good review comment:** > Please keep pricing rules in the Application layer; the controller currently delegates at lines 27-39 so business behavior stays unit-testable.

## Kata 2: Add Worker Detail Route Without Auth
**Resembles:** `src/PeblClone.Api/Controllers/WorkforceController.cs:6-40`  
**Expected findings:** Blocking: missing auth/resource permission. Optional: route naming.  
**Good review comment:** > This exposes worker data like names and payroll details; before adding IDs to routes, add an authorization policy and IDOR test.

## Kata 3: Make EF Read Model Singleton
**Resembles:** `src/PeblClone.Infrastructure/DependencyInjection.cs:9-12`  
**Expected findings:** Blocking: DbContext lifetime issue.  
**Good review comment:** > The current singleton works for static seed data, but an EF adapter should be scoped because DbContext is scoped.

## Kata 4: Add New Enum Value Only In C#
**Resembles:** `src/PeblClone.Domain/Workforce/EmploymentType.cs:1-8`, `src/PeblClone.Web/src/services/workforceTypes.ts:1`  
**Expected findings:** Important: contract drift.  
**Good review comment:** > Please update TS contract, quote form options, pricing switch, and tests with the new employment type.

## Kata 5: Remove `isMounted` Guard
**Resembles:** `src/PeblClone.Web/src/App.tsx:14-35`  
**Expected findings:** Important: possible state update after unmount.  
**Good review comment:** > Keep cleanup behavior or replace it with AbortController; the current effect guards late state updates.

## Kata 6: Add Mock Data To Fix Production Error
**Resembles:** `src/PeblClone.Web/src/services/workforceApi.ts:29-34`  
**Expected findings:** Blocking: hides outage.  
**Good review comment:** > A mock fallback is useful for demos, but production errors need visibility. Add an error state or env-gated fallback.

## Kata 7: Add Payroll Approval Button Without Test
**Resembles:** `src/PeblClone.Web/src/App.tsx:203-223`, `src/PeblClone.Domain/Workforce/PayrollRun.cs:1-10`  
**Expected findings:** Blocking: mutation needs API, auth, idempotency, tests.  
**Good review comment:** > Approval is a side effect. Please design command endpoint, permission check, idempotency behavior, and regression coverage.

## Kata 8: Compare Money By Amount Only
**Resembles:** `src/PeblClone.Domain/Workforce/Money.cs:7-14`  
**Expected findings:** Important: currency correctness.  
**Good review comment:** > Money operations must respect currency; current Add throws on mismatch, so comparisons should also define currency behavior.

## Kata 9: CI Removes Tests To Speed Up Build
**Resembles:** `.github/workflows/ci.yml:16-18`  
**Expected findings:** Blocking: safety regression.  
**Good review comment:** > Build speed matters, but removing tests weakens the contribution contract. Let’s measure and split slow tests instead.

## Kata 10: Dashboard Adds Client-Side Sorting Only
**Resembles:** `src/PeblClone.Web/src/App.tsx:132-152`, `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:32-50`  
**Expected findings:** Important: server/client sorting source of truth.  
**Good review comment:** > Decide whether sort order is product behavior from the API or local UI state; test the chosen owner.

