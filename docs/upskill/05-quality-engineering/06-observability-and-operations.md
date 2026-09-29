# 06 Observability And Operations

## Current Operations Surface

| Concern | Evidence | Gap |
| --- | --- | --- |
| Health check | `src/PeblClone.Api/Program.cs:27,40` | No dependency-specific checks. |
| ProblemDetails | `src/PeblClone.Api/Program.cs:26` and controller 400 at `src/PeblClone.Api/Controllers/WorkforceController.cs:31-36` | No global exception example. |
| Security headers | `src/PeblClone.Api/Extensions/SecurityHeadersExtensions.cs:7-15` | No CSP/HSTS. |
| CI | `.github/workflows/ci.yml:8-33` | No deployment/rollback pipeline. |
| Logging | No `ILogger` usage found | Add structured logs around major flows. |
| Metrics/tracing | No OpenTelemetry found | Add request/use-case metrics later. |

## “How Would I Know This Broke?”

| Flow | Signal to add |
| --- | --- |
| Dashboard | Count API failures before mock fallback at `src/PeblClone.Web/src/services/workforceApi.ts:29-34`; server duration around `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:7-51`. |
| Quote | Log validation failure reason from `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:12-34`; metric quote success/failure. |
| Payroll | Future approval command should log actor, country, run ID; current display is `src/PeblClone.Web/src/App.tsx:203-223`. |
| Compliance | Track high-risk open items from `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:42-46`. |

## Drill

Add an observability design note for `CreateQuote`: what log fields are safe, what fields are sensitive, and what metric would page someone?

