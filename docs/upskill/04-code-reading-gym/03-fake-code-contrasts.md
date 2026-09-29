# 03 Fake Code Contrasts

Each bad example is illustrative fake code: not from this repo.

## Contrast 1: Business Logic In Controller
```csharp
// Illustrative fake code: not from this repo.
var total = request.MonthlyCompensation * 1.21m + 699m;
return Ok(total);
```
Better pattern: controller delegates at `src/PeblClone.Api/Controllers/WorkforceController.cs:27-39`; calculator owns rules at `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:36-65`.

## Contrast 2: Leaking IQueryable
```csharp
// Illustrative fake code: not from this repo.
IQueryable<Worker> GetWorkers();
```
Better pattern: read model returns materialized read data at `src/PeblClone.Application/Workforce/IWorkforceReadModel.cs:7-11`. Avoid leaking persistence query semantics upward.

## Contrast 3: Missing Permission Check
```csharp
// Illustrative fake code: not from this repo.
[HttpGet("{tenantId}/dashboard")]
public Task<Dashboard> Get(string tenantId) => service.Get(tenantId);
```
Current repo has no auth at `src/PeblClone.Api/Controllers/WorkforceController.cs:6-40`; future tenant routes must add authorization.

## Contrast 4: N+1 Compliance Checks
```csharp
// Illustrative fake code: not from this repo.
foreach (var worker in workers) await db.ComplianceChecks.Where(c => c.WorkerId == worker.Id).ToListAsync();
```
Current domain uses `ComplianceChecks` in `src/PeblClone.Domain/Workforce/Worker.cs:20-22`; EF migration must load checks intentionally.

## Contrast 5: Swallowed API Error
```ts
// Illustrative fake code: not from this repo.
catch { return defaultDashboard; }
```
Repo has a real fallback at `src/PeblClone.Web/src/services/workforceApi.ts:29-34`; better production behavior would expose an error state.

## Contrast 6: Side Effect In Mapper
```csharp
// Illustrative fake code: not from this repo.
private WorkerDto MapWorker(Worker w) { email.Send(...); return dto; }
```
Current mapper is pure at `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:54-70`. Keep side effects out of mapping.

## Contrast 7: Unsafe Currency Addition
```csharp
// Illustrative fake code: not from this repo.
return new Money(a.Amount + b.Amount, a.Currency);
```
Better pattern: currency guard at `src/PeblClone.Domain/Workforce/Money.cs:7-14`.

## Contrast 8: Index Key For List Rows
```tsx
// Illustrative fake code: not from this repo.
workers.map((worker, index) => <tr key={index}>...</tr>)
```
Better pattern: stable worker ID key at `src/PeblClone.Web/src/App.tsx:132-133`.

## Contrast 9: Public Contract Change Without TS Update
```csharp
// Illustrative fake code: not from this repo.
public sealed record WorkerSummaryDto(Guid Id, string DisplayName);
```
Current C# DTO is `src/PeblClone.Application/Workforce/DashboardDtos.cs:21-32`; TS mirror is `src/PeblClone.Web/src/services/workforceTypes.ts:25-37`.

## Contrast 10: Captive Dependency
```csharp
// Illustrative fake code: not from this repo.
services.AddSingleton<IReadModel, EfReadModel>();
```
Current singleton is safe only because the adapter is in-memory at `src/PeblClone.Infrastructure/DependencyInjection.cs:11` and `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:6-172`.

## Contrast 11: No Cancellation
```csharp
// Illustrative fake code: not from this repo.
await readModel.GetWorkersAsync(CancellationToken.None);
```
Current service passes the incoming token at `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:9-11`.

## Contrast 12: Test Only The Mock
```ts
// Illustrative fake code: not from this repo.
expect(mockDashboard.summary.totalWorkers).toBe(5);
```
Better existing backend tests assert behavior through services at `tests/PeblClone.Application.Tests/WorkforceDashboardServiceTests.cs:10-22`.

