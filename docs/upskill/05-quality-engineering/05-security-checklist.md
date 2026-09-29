# 05 Security Checklist

Use before merging API or UI changes.

| Check | Current evidence | Status |
| --- | --- | --- |
| Authentication | No auth services in `src/PeblClone.Api/Program.cs:7-27` | Missing |
| Authorization | No `[Authorize]` at `src/PeblClone.Api/Controllers/WorkforceController.cs:6-40` | Missing |
| IDOR | No ID routes yet; future routes would start from `src/PeblClone.Api/Controllers/WorkforceController.cs:7` | Future risk |
| Input validation | Quote validates compensation/country/type at `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:12-34` | Partial |
| XSS | React escapes text rendered in `src/PeblClone.Web/src/App.tsx:132-152`; no `dangerouslySetInnerHTML` found | Low current risk |
| CSRF | API appears stateless; no cookies/anti-forgery configured in `src/PeblClone.Api/Program.cs:7-41` | Investigate if cookies added |
| SQL injection | No SQL/EF found; in-memory adapter at `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:6-172` | Not applicable now |
| SSRF | No server-side outbound HTTP found | Not applicable now |
| Secrets | `.env.example` only has API base URL at `src/PeblClone.Web/.env.example:1` | No secrets found |
| CORS | Local origins only at `src/PeblClone.Api/Program.cs:7-18` | Good for dev, needs env config |
| Headers | Four headers at `src/PeblClone.Api/Extensions/SecurityHeadersExtensions.cs:7-15` | Partial |
| Rate limiting | No rate limiter registration at `src/PeblClone.Api/Program.cs:7-27` | Missing |
| Dependency risk | npm lock generated; package versions in `src/PeblClone.Web/package.json:12-24` | Needs ongoing audit |

Pre-merge question: “Can a caller access, mutate, or infer payroll/worker data they should not see?” Anchor payroll and worker fields at `src/PeblClone.Application/Workforce/DashboardDtos.cs:13-32`.

