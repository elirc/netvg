# 02 Trace Tables

## Trace 1: UI To Dashboard API

| Step | File | Value shape | Owner | Transformation | Risk |
| --- | --- | --- | --- | --- | --- |
| 1 | `src/PeblClone.Web/src/App.tsx:14-35` | state empty -> dashboard | React | effect fetches data | no explicit error UI |
| 2 | `src/PeblClone.Web/src/services/workforceApi.ts:23-35` | JSON or mock | client service | fetch or fallback | outage hidden |
| 3 | `src/PeblClone.Api/Controllers/WorkforceController.cs:12-18` | HTTP -> DTO | API | delegates | no auth |
| 4 | `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:9-51` | domain lists -> dashboard DTO | Application | aggregates/maps | unbounded reads |
| 5 | `src/PeblClone.Web/src/App.tsx:84-227` | DTO -> DOM | React | renders sections | no pagination |

## Trace 2: Quote Failure

| Step | File | Value shape | Owner | Transformation | Risk |
| --- | --- | --- | --- | --- | --- |
| 1 | `src/PeblClone.Web/src/features/QuoteBuilder.tsx:22-27` | request | React | submits selected form state | can bypass HTML min |
| 2 | `src/PeblClone.Api/Controllers/WorkforceController.cs:23-27` | request body | API | model binding + delegate | no annotations |
| 3 | `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:12-34` | `Result.Failure` | Application | validates | message is string |
| 4 | `src/PeblClone.Api/Controllers/WorkforceController.cs:29-36` | ProblemDetails | API | 400 response | no standardized error code |
| 5 | `src/PeblClone.Web/src/services/workforceApi.ts:43-77` | fallback quote | client service | catches error | real validation failure may be hidden |

## Trace 3: In-Memory Persistence Adapter

| Step | File | Value shape | Owner | Transformation | Risk |
| --- | --- | --- | --- | --- | --- |
| 1 | `src/PeblClone.Infrastructure/DependencyInjection.cs:9-12` | service registration | Infrastructure | binds interface | singleton lifetime |
| 2 | `src/PeblClone.Application/Workforce/IWorkforceReadModel.cs:5-12` | port | Application | abstracts reads | can overfit to dashboard |
| 3 | `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:10-162` | seed lists | Infrastructure | static data | not durable |
| 4 | `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:164-171` | completed tasks | Infrastructure | returns lists | ignores cancellation |

## Trace 4: Permission Boundary Gap

| Step | File | Value shape | Owner | Transformation | Risk |
| --- | --- | --- | --- | --- | --- |
| 1 | `src/PeblClone.Api/Program.cs:7-27` | services | API | no auth services registered | unauthenticated access |
| 2 | `src/PeblClone.Api/Controllers/WorkforceController.cs:6-7` | attributes | API | no `[Authorize]` | payroll/worker data unprotected |
| 3 | `src/PeblClone.Application/Workforce/DashboardDtos.cs:13-32` | payroll + worker names | Application | response DTO | sensitive data if real |

## Trace 5: CI

| Step | File | Value shape | Owner | Transformation | Risk |
| --- | --- | --- | --- | --- | --- |
| 1 | `.github/workflows/ci.yml:3-6` | event | GitHub Actions | starts on PR/push | no schedule/security scan |
| 2 | `.github/workflows/ci.yml:13-18` | solution | backend job | restore/build/test | local SDK missing |
| 3 | `.github/workflows/ci.yml:27-33` | frontend | frontend job | npm ci/build | lint script not run |

## Trace 6: Security Headers

| Step | File | Value shape | Owner | Transformation | Risk |
| --- | --- | --- | --- | --- | --- |
| 1 | `src/PeblClone.Api/Program.cs:31` | middleware | API | installs headers | runs before endpoints |
| 2 | `src/PeblClone.Api/Extensions/SecurityHeadersExtensions.cs:7-15` | response headers | middleware | adds four headers | no CSP/HSTS |
| 3 | `src/PeblClone.Api/Program.cs:38-41` | request pipeline | API | HTTPS/CORS/health/controllers | production config unknown |

