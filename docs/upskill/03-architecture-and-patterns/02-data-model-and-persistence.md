# 02 Data Model And Persistence

## Current State

No EF Core `DbContext`, migration folder, connection string, or repository implementation was found. The persistence stand-in is `InMemoryWorkforceReadModel`, registered as the `IWorkforceReadModel` adapter at `src/PeblClone.Infrastructure/DependencyInjection.cs:9-12`.

Current model anchors:

- Workers: `src/PeblClone.Domain/Workforce/Worker.cs:3-23`.
- Countries: `src/PeblClone.Domain/Workforce/CountryProfile.cs:3-13`.
- Compliance checks: `src/PeblClone.Domain/Workforce/ComplianceCheck.cs:1-10`.
- Payroll runs: `src/PeblClone.Domain/Workforce/PayrollRun.cs:1-10`.
- Seeded country data: `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:10-67`.
- Seeded worker data: `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:69-154`.
- Seeded payroll data: `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:156-162`.

## How EF Core Would Fit

Illustrative fake code: not from this repo.

```csharp
// Illustrative fake code: not from this repo.
public sealed class WorkforceDbContext : DbContext
{
    public DbSet<WorkerEntity> Workers => Set<WorkerEntity>();
}
```

If EF is added, keep the application port stable at `src/PeblClone.Application/Workforce/IWorkforceReadModel.cs:5-12`. Replace `InMemoryWorkforceReadModel` with an EF adapter and update DI at `src/PeblClone.Infrastructure/DependencyInjection.cs:9-12`.

## Migration Guidance

1. Add entities that map to domain concepts without letting EF attributes leak into Domain.
2. Add a scoped DbContext, not a singleton adapter; current singleton evidence is `src/PeblClone.Infrastructure/DependencyInjection.cs:11`.
3. Add migrations and index country/status fields used for dashboard filters and joins.
4. Add integration tests around query shape and summary counts.
5. Keep rollback possible: deploy schema, then code, then data changes.

## Failure Modes

- N+1: if workers load compliance checks lazily, `RequiresAttention` at `src/PeblClone.Domain/Workforce/Worker.cs:20-22` can trigger many queries.
- Unbounded queries: dashboard currently reads all workers at `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:10`.
- Time instability: seed data uses `DateTime.UtcNow` at `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:8`; production tests need injectable time.

## Drill

Design an EF-backed `IWorkforceReadModel` that returns exactly the same DTOs. Which tests from `tests/PeblClone.Application.Tests/WorkforceDashboardServiceTests.cs:10-22` should still pass?

