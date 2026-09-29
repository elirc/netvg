# 01 Code Review Mindset

Review in this order:

1. Does it work?
2. Is it correct?
3. Will it stay correct?
4. Does it fit the codebase?
5. Is it kind to maintainers?

## Repo Checklist

| Question | Anchor |
| --- | --- |
| Did API behavior stay in controllers or use cases appropriately? | `src/PeblClone.Api/Controllers/WorkforceController.cs:27-39`, `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:12-65` |
| Did a DTO change update TypeScript contracts? | `src/PeblClone.Application/Workforce/DashboardDtos.cs:5-68`, `src/PeblClone.Web/src/services/workforceTypes.ts:7-97` |
| Did tests cover the changed rule? | `tests/PeblClone.Application.Tests/HiringQuoteCalculatorTests.cs:11-37` |
| Did a persistence change respect the port? | `src/PeblClone.Application/Workforce/IWorkforceReadModel.cs:5-12` |
| Did a UI data change handle loading/error states? | `src/PeblClone.Web/src/App.tsx:42-47`, `src/PeblClone.Web/src/services/workforceApi.ts:23-35` |

Good review comment:

> This works locally, but it moves pricing policy into the browser. The backend currently owns quote policy at `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:36-65`; can we keep the UI as display-only or add a contract test?

