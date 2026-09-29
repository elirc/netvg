# 01 Annotation Drills

For each drill, annotate inputs, outputs, dependencies, invariants, side effects, failure modes, and one test idea.

| Drill | Code | Prompt | Self-grade |
| --- | --- | --- | --- |
| 1 Dashboard service | `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:7-51` | Mark every collection materialization and sort. | Basic names steps; Solid explains data shape; Strong identifies scaling risk. |
| 2 Worker invariant | `src/PeblClone.Domain/Workforce/Worker.cs:15-22` | Explain when progress is 100 and when attention is true. | Strong adds high-risk compliance test idea. |
| 3 Quote validation | `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:12-34` | Separate input validation from business availability validation. | Strong proposes missing currency validation. |
| 4 Quote pricing | `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:36-65` | Identify policy constants and output fields. | Strong notices hard-coded rates. |
| 5 Controller errors | `src/PeblClone.Api/Controllers/WorkforceController.cs:27-39` | Trace `Result` failure to `ProblemDetails`. | Strong asks for API integration tests. |
| 6 DI lifetimes | `src/PeblClone.Application/DependencyInjection.cs:8-12`, `src/PeblClone.Infrastructure/DependencyInjection.cs:9-12` | Explain scoped vs singleton in this repo. | Strong warns about EF DbContext captive dependency. |
| 7 Fetch wrapper | `src/PeblClone.Web/src/services/workforceApi.ts:6-21` | Mark HTTP boundary and error path. | Strong proposes typed runtime validation. |
| 8 Mock fallback | `src/PeblClone.Web/src/services/workforceApi.ts:23-35,43-77` | Identify duplicated business behavior. | Strong suggests env-gated fallback. |
| 9 React effect | `src/PeblClone.Web/src/App.tsx:14-35` | Explain cleanup guard and stale closure risk. | Strong proposes AbortController. |
| 10 Quote form | `src/PeblClone.Web/src/features/QuoteBuilder.tsx:17-31,43-78` | Trace form state to request body. | Strong distinguishes client constraint from server validation. |
| 11 CI | `.github/workflows/ci.yml:9-33` | Mark backend and frontend gates. | Strong identifies missing lint/API/UI tests. |
| 12 Security headers | `src/PeblClone.Api/Extensions/SecurityHeadersExtensions.cs:7-15` | Explain what each header does and what is missing. | Strong separates API headers from browser app CSP. |

