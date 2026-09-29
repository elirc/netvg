# Learning Rubrics

## Junior

Observable behaviors:

- Can run documented commands or identify why they cannot run, using `08-reference/command-cheatsheet.md`.
- Can trace dashboard flow from `src/PeblClone.Web/src/App.tsx:14-35` to `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:7-51`.
- Can add a focused application test beside `tests/PeblClone.Application.Tests/HiringQuoteCalculatorTests.cs:11-37`.
- Can avoid changing unrelated files.

Self-check:

- [ ] I can name API, Application, Domain, Infrastructure, Web, Tests.
- [ ] I can explain one invariant from `src/PeblClone.Domain/Workforce/Worker.cs:20-22`.
- [ ] I can identify mock data vs API data.

## Mid-Level

Observable behaviors:

- Designs cross-layer changes with API, Application, UI, and tests.
- Spots contract drift between `src/PeblClone.Application/Workforce/DashboardDtos.cs:5-68` and `src/PeblClone.Web/src/services/workforceTypes.ts:7-97`.
- Adds API or UI tests for behavior, not implementation.
- Explains DI lifetime implications from `src/PeblClone.Application/DependencyInjection.cs:8-12` and `src/PeblClone.Infrastructure/DependencyInjection.cs:9-12`.

Self-check:

- [ ] I can write a PR description with risks and tests.
- [ ] I can explain why an EF adapter should not be singleton.
- [ ] I can debug a failed quote flow layer by layer.

## Senior

Observable behaviors:

- Identifies blast radius, rollback, migration, and observability needs before coding.
- Treats missing auth at `src/PeblClone.Api/Controllers/WorkforceController.cs:6-40` as a production readiness issue.
- Proposes incremental migration from `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:6-172` to durable persistence.
- Reviews PRs for long-term ownership, not just local correctness.

Self-check:

- [ ] I can write an RFC for payroll approval.
- [ ] I can rank risks by impact and likelihood.
- [ ] I can explain who pays for a design tradeoff.

