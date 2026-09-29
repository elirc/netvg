# 02 Writing PRs And RFCs

## PR Template

```md
## What
[Short description]

## Why
[User or maintainer value]

## How tested
- [ ] dotnet test PeblClone.sln
- [ ] npm run build
- [ ] Manual: [flow]

## Risks
[Auth, persistence, contract, UI, perf]

## Follow-ups
[Explicitly out of scope]
```

Use command anchors: backend CI at `.github/workflows/ci.yml:16-18`, frontend CI at `.github/workflows/ci.yml:32-33`.

## Commit Messages

Good: `test: cover negative quote compensation` for a change around `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:12-15`.

Weak: `fix stuff`.

## When To RFC

Write an RFC for changes that affect architecture, public contracts, security, persistence, or production operations. Examples:

- Adding auth to endpoints at `src/PeblClone.Api/Controllers/WorkforceController.cs:6-40`.
- Replacing in-memory data at `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:6-172`.
- Generating TS contracts from C# DTOs at `src/PeblClone.Application/Workforce/DashboardDtos.cs:5-68`.

