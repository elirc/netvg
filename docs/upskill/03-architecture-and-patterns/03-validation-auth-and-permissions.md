# 03 Validation, Auth, And Permissions

## Validation

Current validation lives in the application service:

- Non-positive compensation rejected at `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:12-15`.
- Unknown country rejected at `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:17-24`.
- Unsupported EOR/contractor availability rejected at `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:26-34`.
- Controller turns failed `Result` into `ProblemDetails` at `src/PeblClone.Api/Controllers/WorkforceController.cs:29-36`.

Junior misses: HTML input minimum at `src/PeblClone.Web/src/features/QuoteBuilder.tsx:66-72` is not enough; clients can bypass it.

Senior checks: distinguish input shape validation, business validation, and authorization. `HiringQuoteRequest` has no attributes at `src/PeblClone.Application/Workforce/HiringQuoteDtos.cs:5-9`, so shape validation is minimal.

## Auth And Permissions

No authentication or authorization boundary was found:

- Controller has `[ApiController]` and `[Route]`, not `[Authorize]`, at `src/PeblClone.Api/Controllers/WorkforceController.cs:6-7`.
- Program registers controllers, CORS, health, OpenAPI, but not auth services at `src/PeblClone.Api/Program.cs:7-27`.

Possible risk, not a confirmed bug: if this API were exposed beyond local/demo use, workforce, payroll, and compliance data would need authentication, tenant scoping, and resource authorization.

## Tenant And IDOR Thinking

IDOR means “insecure direct object reference”: a caller can access a resource by changing an ID. Current endpoints do not accept worker IDs, but future quote/workforce detail endpoints will. Use current route style at `src/PeblClone.Api/Controllers/WorkforceController.cs:7-24` as the place where resource IDs would enter.

## Drill

Design an authorization policy for `GET /api/workforce/dashboard`. Who can view payroll totals from `src/PeblClone.Application/Workforce/DashboardDtos.cs:13-19`? Who can view worker names from `src/PeblClone.Application/Workforce/DashboardDtos.cs:21-32`?

