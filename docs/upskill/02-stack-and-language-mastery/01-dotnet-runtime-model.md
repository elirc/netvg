# 01 .NET Runtime Model

## CLR, JIT, And Boundaries

The API runs as an ASP.NET Core process created at `src/PeblClone.Api/Program.cs:5-43`. The CLR loads compiled assemblies for API, Application, Domain, and Infrastructure; the JIT compiles methods such as `WorkforceDashboardService.GetDashboardAsync` at `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:7-51` when executed.

Why it matters: senior engineers think about startup, warm paths, allocation rate, and where latency enters. This repo is tiny, but the same habits apply when `GetCountriesAsync`, `GetWorkersAsync`, and `GetPayrollRunsAsync` at `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:9-11` become database or network calls.

## GC And Allocations

Representative allocations:

- `ToDictionary` allocates a dictionary at `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:12`.
- `ToList` materializes compliance items at `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:14-17`.
- Mapping workers/countries/payroll into DTO lists allocates at `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:32-50`.
- Records such as `Worker` and `Money` are reference types here; see `src/PeblClone.Domain/Workforce/Worker.cs:3-13` and `src/PeblClone.Domain/Workforce/Money.cs:3-15`.

Failure modes:

- Unbounded reads can create large allocations if the worker list grows and no pagination is added.
- Recomputing dashboard projections on every request is fine for seed data but expensive with real data.

Drill: Estimate the number of lists created in `GetDashboardAsync` using `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:9-51`.

## Async, Task, And Cancellation

The public controller actions are async at `src/PeblClone.Api/Controllers/WorkforceController.cs:14-17` and `src/PeblClone.Api/Controllers/WorkforceController.cs:23-39`. They accept `CancellationToken`, and the application passes it to the read model at `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:9-11`.

Why it matters: cancellation is a contract. If the browser disconnects or an upstream timeout fires, future database or HTTP calls should stop. The current adapter ignores the token because it returns in-memory data at `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:164-171`.

Failure modes:

- Sync-over-async deadlocks: not present; no `.Result` or `.Wait()` found in the inspected code.
- Serial awaits: `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:9-11` are serial. That is acceptable for an in-memory adapter; investigate whether parallelism is useful after persistence exists.
- `ValueTask`: not used. Use it only when measurements show hot paths usually complete synchronously.

Drill: If `GetWorkersAsync` becomes a SQL query, where should cancellation be passed? Start at `src/PeblClone.Application/Workforce/IWorkforceReadModel.cs:7-11`.

## IDisposable And Resource Ownership

No explicit `IDisposable`, `using`, or external resource ownership was found. Future EF Core or HTTP clients should be owned through DI, not manually newed in application services. Compare current DI registration at `src/PeblClone.Application/DependencyInjection.cs:8-13` and `src/PeblClone.Infrastructure/DependencyInjection.cs:9-12`.

## Verification Notes

Inspected API actions, Application async methods, read model adapter, and domain records. No spans, `ValueTask`, `ConfigureAwait`, `IDisposable`, or sync-over-async usage found.

