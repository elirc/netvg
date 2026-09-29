# 03 Senior Build Projects

## Project 1: Multi-Tenant Authorization
**Problem:** Workforce/payroll data needs tenant isolation before real use.  
**Product value:** Customers see only their data.  
**Design checklist:** authn, authz policies, tenant ID source, IDOR tests, audit.  
**Anchors:** no current auth at `src/PeblClone.Api/Program.cs:7-27`; sensitive DTO fields at `src/PeblClone.Application/Workforce/DashboardDtos.cs:13-32`.  
**Rollout/rollback:** introduce policies in warn-only/dev, then enforce.

## Project 2: Durable Persistence
**Problem:** Seed data is not durable.  
**Product value:** Real customer workforce data persists.  
**Anchors:** current adapter `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:6-172`; port `src/PeblClone.Application/Workforce/IWorkforceReadModel.cs:5-12`.  
**Plan:** add EF adapter, migrations, integration tests, indexes, rollback scripts.

## Project 3: Contract-Generated Frontend Client
**Problem:** C# DTOs and TS interfaces can drift.  
**Anchors:** C# DTOs `src/PeblClone.Application/Workforce/DashboardDtos.cs:5-68`; TS interfaces `src/PeblClone.Web/src/services/workforceTypes.ts:7-97`.  
**Plan:** OpenAPI generation, compile gate, migration from manual client.

## Project 4: Payroll Approval Workflow
**Problem:** Payroll queue is display-only.  
**Anchors:** domain record `src/PeblClone.Domain/Workforce/PayrollRun.cs:1-10`; UI display `src/PeblClone.Web/src/App.tsx:203-223`.  
**Plan:** command endpoint, idempotency key, authorization, audit log, tests, rollback.

## Project 5: Observability Foundation
**Problem:** Health exists, but no structured logs/metrics/traces.  
**Anchors:** health registration `src/PeblClone.Api/Program.cs:27,40`; no `ILogger` usage found.  
**Plan:** add logging scopes, metrics per flow, dashboard alerts, runbook.

## Project 6: Production Error Handling
**Problem:** Mock fallback hides API failures.  
**Anchors:** fallback `src/PeblClone.Web/src/services/workforceApi.ts:29-34,43-77`; source badge `src/PeblClone.Web/src/App.tsx:69-72`.  
**Plan:** env-gated demo mode, explicit errors, E2E tests, telemetry.

