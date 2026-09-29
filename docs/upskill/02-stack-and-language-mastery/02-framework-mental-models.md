# 02 Framework Mental Models

## ASP.NET Core Pipeline

Concept: ASP.NET Core requests flow through middleware in order, then endpoint routing invokes controllers.

Real code:

- Services are registered before `builder.Build()` at `src/PeblClone.Api/Program.cs:7-27`.
- Middleware/endpoints are configured after build at `src/PeblClone.Api/Program.cs:31-41`.
- Controllers are discovered because `AddControllers` is called at `src/PeblClone.Api/Program.cs:24` and mapped at `src/PeblClone.Api/Program.cs:41`.

Why it matters: middleware order is a contract. Security headers are added before routing at `src/PeblClone.Api/Program.cs:31`; CORS runs before controllers at `src/PeblClone.Api/Program.cs:39-41`.

Failure modes:

- Putting CORS after endpoints can make preflight requests fail.
- Mapping OpenAPI outside the development guard at `src/PeblClone.Api/Program.cs:33-36` may expose metadata unexpectedly.

Drill: Draw a request path for `GET /api/workforce/dashboard` using `src/PeblClone.Api/Program.cs:31-41` and `src/PeblClone.Api/Controllers/WorkforceController.cs:12-18`.

## Dependency Injection Lifetimes

Real code:

- `WorkforceDashboardService` and `IHiringQuoteCalculator` are scoped at `src/PeblClone.Application/DependencyInjection.cs:8-12`.
- `IWorkforceReadModel` is singleton at `src/PeblClone.Infrastructure/DependencyInjection.cs:9-12`.
- The controller receives dependencies via primary constructor at `src/PeblClone.Api/Controllers/WorkforceController.cs:8-10`.

Why it matters: lifetimes determine ownership. Singleton depending on scoped services is a captive dependency bug. This repo avoids it because singleton `InMemoryWorkforceReadModel` has only static in-memory data at `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:8-171`.

Failure modes:

- If the read model becomes EF-backed, do not keep it singleton; a `DbContext` should be scoped.
- If a scoped service stores mutable per-user state, singleton consumers can leak tenant data.

## Model Binding And Validation

`CreateQuote` binds JSON from the request body at `src/PeblClone.Api/Controllers/WorkforceController.cs:23-25`. Application validation occurs at `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:12-34`.

Why it matters: model binding creates objects; validation decides whether the object is allowed. Currently, `HiringQuoteRequest` is a positional record with no data annotations at `src/PeblClone.Application/Workforce/HiringQuoteDtos.cs:5-9`.

Drill: Propose where to validate a missing or empty `CountryIsoCode`. Distinguish HTTP shape validation from business validation.

## React Render Model

The React app renders from state:

- `dashboard`, `source`, and `isLoading` are state variables at `src/PeblClone.Web/src/App.tsx:10-12`.
- Data fetching happens in an effect at `src/PeblClone.Web/src/App.tsx:14-35`.
- Derived alert filtering uses `useMemo` at `src/PeblClone.Web/src/App.tsx:37-40`.
- List keys are worker IDs at `src/PeblClone.Web/src/App.tsx:132-133` and country ISO codes at `src/PeblClone.Web/src/App.tsx:186-187`.

Why it matters: render should be a pure function of state and props. Side effects belong in `useEffect` or event handlers, as the quote form does at `src/PeblClone.Web/src/features/QuoteBuilder.tsx:17-31`.

Failure modes:

- Stale closures: less risky here because `useEffect` has an empty dependency array at `src/PeblClone.Web/src/App.tsx:35`, but future props would need dependencies.
- State after unmount: guarded by `isMounted` at `src/PeblClone.Web/src/App.tsx:15-34`; an `AbortController` would be stronger.
- Unbounded list rendering: worker rows are rendered directly at `src/PeblClone.Web/src/App.tsx:132-152`.

## Verification Notes

No React Router, React Query, Server Components, filters, or custom ASP.NET model validators were found.

