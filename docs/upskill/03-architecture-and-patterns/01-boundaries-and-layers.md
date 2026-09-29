# 01 Boundaries And Layers

## Ownership Rules

| Layer | Owns | Must not own | Evidence |
| --- | --- | --- | --- |
| API | HTTP, middleware, request/response status | Business math, storage details | `src/PeblClone.Api/Controllers/WorkforceController.cs:12-40` delegates. |
| Application | Use cases, DTOs, ports | React UI, concrete persistence | `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:7-51`, `src/PeblClone.Application/Workforce/IWorkforceReadModel.cs:5-12` |
| Domain | Invariants and business nouns | HTTP, DI, storage | `src/PeblClone.Domain/Workforce/Worker.cs:15-22`, `src/PeblClone.Domain/Workforce/Money.cs:7-14` |
| Infrastructure | Adapter implementations | Use-case orchestration | `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:6-172` |
| Web | UI state, forms, browser API calls | Server business truth | `src/PeblClone.Web/src/App.tsx:14-40`, `src/PeblClone.Web/src/features/QuoteBuilder.tsx:17-31` |

## Boundary Leaks

- Possible leak: frontend mock quote duplicates backend pricing rules at `src/PeblClone.Web/src/services/workforceApi.ts:50-75` while backend rules live at `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:36-65`.
- Possible leak: TS contracts are manually mirrored at `src/PeblClone.Web/src/services/workforceTypes.ts:7-97` from C# DTOs at `src/PeblClone.Application/Workforce/DashboardDtos.cs:5-68`.
- Not a leak: API delegates quote creation instead of calculating totals itself at `src/PeblClone.Api/Controllers/WorkforceController.cs:27-39`.

## Drill

For a new “approve payroll run” feature, decide which layer owns: route, command DTO, approval invariant, database update, audit event, and UI button. Anchor your answer to existing payroll display at `src/PeblClone.Web/src/App.tsx:203-223` and payroll DTO mapping at `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:96-107`.

