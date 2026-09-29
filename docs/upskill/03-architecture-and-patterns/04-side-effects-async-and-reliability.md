# 04 Side Effects, Async, And Reliability

## Current Async Flow

The app uses async request handling but no background queue:

- Controller actions are async at `src/PeblClone.Api/Controllers/WorkforceController.cs:14-17` and `src/PeblClone.Api/Controllers/WorkforceController.cs:23-39`.
- Application reads are awaited at `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:9-11`.
- The in-memory adapter returns completed tasks at `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:164-171`.
- Frontend fetch uses promises at `src/PeblClone.Web/src/services/workforceApi.ts:6-21`.

No `IHostedService`, `BackgroundService`, queue, webhook, `IHttpClientFactory`, Polly retry policy, cache, or outbox was found.

## Reliability Concepts

| Concept | How to teach it here |
| --- | --- |
| Idempotency | Future payroll approval should tolerate duplicate requests; current payroll is display-only at `src/PeblClone.Web/src/App.tsx:203-223`. |
| Timeout | Future external payroll/country APIs should honor cancellation from `src/PeblClone.Api/Controllers/WorkforceController.cs:14,25`. |
| Retry | Safe only for idempotent reads/commands; current read model has no side effects at `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:164-171`. |
| Outbox | Future compliance events should be persisted before publishing; no eventing exists today. |
| Failure visibility | Current mock fallback at `src/PeblClone.Web/src/services/workforceApi.ts:29-34` can hide API failures. |

## Drill

Design a background job that refreshes country compliance scores. Where would the hosted service live, which Application port would it call, and what should happen if one country fails?

