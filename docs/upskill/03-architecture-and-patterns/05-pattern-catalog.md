# 05 Pattern Catalog

## Pattern: Composition Root
**Problem:** Keep runtime wiring in one place.  
**Real example:** `src/PeblClone.Api/Program.cs:5-43`  
**Why it works:** Startup owns DI and middleware, not controllers.  
**Failure modes:** Hidden service registration in random projects.  
**Drill:** Add where authentication would be registered.

## Pattern: Thin Controller
**Problem:** HTTP code should not own business rules.  
**Real example:** `src/PeblClone.Api/Controllers/WorkforceController.cs:12-40`  
**Why it works:** Controller delegates to use cases.  
**Failure modes:** Money math in controller makes testing harder.  
**Drill:** Name the line where quote business logic is delegated.

## Pattern: Application Service
**Problem:** Coordinate data and domain rules for a use case.  
**Real example:** `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:7-51`  
**Why it works:** UI receives one composed DTO.  
**Failure modes:** Service grows into a god object.  
**Drill:** Split one private mapper into a separate collaborator only if duplication appears.

## Pattern: Port Interface
**Problem:** Application should not know storage details.  
**Real example:** `src/PeblClone.Application/Workforce/IWorkforceReadModel.cs:5-12`  
**Why it works:** Infrastructure can change without controller changes.  
**Failure modes:** Port leaks EF `IQueryable` into Application.  
**Drill:** Sketch an EF implementation signature.

## Pattern: Adapter Registration
**Problem:** Bind interfaces to implementations at runtime.  
**Real example:** `src/PeblClone.Infrastructure/DependencyInjection.cs:9-12`  
**Why it works:** Composition stays explicit.  
**Failure modes:** Wrong lifetime when adapter gains scoped dependencies.  
**Drill:** Explain why singleton is okay for seed data but risky for DbContext.

## Pattern: Domain Computed Property
**Problem:** Keep reusable invariant close to the data.  
**Real example:** `src/PeblClone.Domain/Workforce/Worker.cs:15-22`  
**Why it works:** Attention logic has one source.  
**Failure modes:** Expensive computed property over lazy-loaded data.  
**Drill:** Add a domain test for high-risk unresolved compliance.

## Pattern: Value Object
**Problem:** Money needs currency-aware behavior.  
**Real example:** `src/PeblClone.Domain/Workforce/Money.cs:3-15`  
**Why it works:** Prevents adding USD to EUR.  
**Failure modes:** Allows negative amount unless separately guarded.  
**Drill:** Propose an invariant for negative payroll.

## Pattern: Result Type
**Problem:** Return success/failure without exceptions for expected validation failures.  
**Real example:** `src/PeblClone.Application/Common/Result.cs:3-10`  
**Why it works:** Controller maps failure to `ProblemDetails`.  
**Failure modes:** Caller ignores `IsSuccess`.  
**Drill:** Find the null guard in `src/PeblClone.Api/Controllers/WorkforceController.cs:29`.

## Pattern: DTO Boundary
**Problem:** Avoid exposing domain records directly to the API.  
**Real example:** `src/PeblClone.Application/Workforce/DashboardDtos.cs:5-68`  
**Why it works:** API shape can differ from domain shape.  
**Failure modes:** Manual TS drift.  
**Drill:** Compare with `src/PeblClone.Web/src/services/workforceTypes.ts:7-97`.

## Pattern: Mock Fallback
**Problem:** Keep demos working without a backend.  
**Real example:** `src/PeblClone.Web/src/services/workforceApi.ts:23-35`  
**Why it works:** UI can load offline.  
**Failure modes:** Production outage masked by mock data.  
**Drill:** Add a config flag design to disable it.

## Pattern: Effect Cleanup Guard
**Problem:** Avoid setting React state after unmount.  
**Real example:** `src/PeblClone.Web/src/App.tsx:14-35`  
**Why it works:** `isMounted` blocks late state updates.  
**Failure modes:** Request still runs; `AbortController` would cancel.  
**Drill:** Rewrite the pattern with abort in fake code.

## Pattern: Typed Client Contract
**Problem:** Let UI code know expected JSON shape.  
**Real example:** `src/PeblClone.Web/src/services/workforceTypes.ts:7-97`  
**Why it works:** Compile-time checks in components.  
**Failure modes:** Runtime JSON can still be wrong.  
**Drill:** Identify one field that appears in both C# and TS.

## Pattern: Central Package Management
**Problem:** Avoid package version sprawl.  
**Real example:** `Directory.Packages.props:1-10`  
**Why it works:** Versions change once.  
**Failure modes:** Unreviewed major bump affects all projects.  
**Drill:** Add where a new package version should go.

## Pattern: CI As Contract
**Problem:** Make expected checks explicit.  
**Real example:** `.github/workflows/ci.yml:9-33`  
**Why it works:** Contributors know what must pass.  
**Failure modes:** Missing lint/security/API tests leaves gaps.  
**Drill:** Propose one new CI job.

## Pattern: Security Header Middleware
**Problem:** Apply cross-cutting HTTP response policy once.  
**Real example:** `src/PeblClone.Api/Extensions/SecurityHeadersExtensions.cs:7-15`  
**Why it works:** Every response gets common headers.  
**Failure modes:** Missing CSP/HSTS; headers may differ by environment.  
**Drill:** Research which headers matter for an API vs a browser app.

