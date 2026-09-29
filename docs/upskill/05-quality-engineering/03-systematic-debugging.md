# 03 Systematic Debugging

Process: reproduce -> narrow layer -> hypothesize -> test cheaply -> fix root cause -> add regression coverage.

## Scenario: Dashboard Shows Demo Fallback
**Reproduction:** Stop API, run web, load dashboard.  
**First question:** API unavailable or client parsing failed?  
**Narrowing path:** Check `src/PeblClone.Web/src/services/workforceApi.ts:23-35`; inspect network request to `/api/workforce/dashboard`; check API route at `src/PeblClone.Api/Controllers/WorkforceController.cs:12-18`.  
**Useful probes:** Browser network panel, console, API logs after adding `ILogger`.  
**Likely root causes:** API not running, wrong `VITE_API_BASE_URL`, CORS.  
**Regression test:** UI test asserting error/fallback badge at `src/PeblClone.Web/src/App.tsx:69-72`.  
**Senior lesson:** Fallbacks need visibility.

## Scenario: Quote Allows Unsupported Contractor In UI
**Reproduction:** Select Japan + Contractor.  
**First question:** Did backend reject or did client fallback hide it?  
**Narrowing path:** Backend rejects at `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:31-34`; client fallback catches errors at `src/PeblClone.Web/src/services/workforceApi.ts:43-77`.  
**Regression test:** API test should expect 400; UI test should show failure when fallback disabled.  
**Senior lesson:** Do not let demo fallback override business validation.

## Scenario: Attention Count Is Wrong
**Reproduction:** Dashboard metric differs from worker list.  
**Narrowing path:** Domain invariant at `src/PeblClone.Domain/Workforce/Worker.cs:20-22`; summary count at `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:23`; worker sort at `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:32-36`; UI metric at `src/PeblClone.Web/src/App.tsx:100-103`.  
**Regression test:** Extend `tests/PeblClone.Application.Tests/WorkforceDashboardServiceTests.cs:10-22`.

## Scenario: CORS Failure From Vite
**Reproduction:** Browser blocks API request.  
**Narrowing path:** Vite port at `src/PeblClone.Web/vite.config.ts:6-9`; CORS origins at `src/PeblClone.Api/Program.cs:7-18`; API URL env at `src/PeblClone.Web/.env.example:1`.  
**Regression test:** API integration test can inspect CORS preflight behavior.

## Scenario: CI Fails Frontend Build
**Reproduction:** PR fails frontend job.  
**Narrowing path:** CI uses Node 22 at `.github/workflows/ci.yml:27-33`; build script runs TypeScript then Vite at `src/PeblClone.Web/package.json:8`.  
**Regression test:** Add local `npm run build` before pushing.

