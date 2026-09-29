# AtlasWork Global Workforce Platform

AtlasWork is a teaching-oriented, production-shaped clone of the core product surface behind modern global workforce platforms such as Velocity Global / Pebl: hire, pay, and manage international employees and contractors from one dashboard.

It is intentionally not a brand copy. The domain, workflows, and UX mirror the common global employment problem space while using original product naming and visual design.

## Product Surface

- Global workforce dashboard with headcount, payroll, compliance, onboarding, and country readiness.
- Employer-of-record and contractor hiring quote workflow.
- Compliance risk scoring for worker classification, payroll timing, benefits, and work authorization.
- Enterprise-style React UI with typed API access, loading states, error states, and deterministic mock fallback data.
- Layered .NET backend using Domain, Application, Infrastructure, and API boundaries.

## Repository Layout

```text
src/
  PeblClone.Domain/          Enterprise rules and value objects
  PeblClone.Application/     Use cases, DTOs, ports
  PeblClone.Infrastructure/  Adapter implementations and seed data
  PeblClone.Api/             HTTP API composition root
  PeblClone.Web/             React + TypeScript frontend
tests/
  PeblClone.Application.Tests/
```

## Run Locally

This machine does not currently have the .NET SDK installed, so no packages were installed or restored during creation.

When the SDK is available:

```powershell
dotnet restore
dotnet test
dotnet run --project src/PeblClone.Api
```

For the web app:

```powershell
cd src/PeblClone.Web
npm install
npm run dev
```

Set `VITE_API_BASE_URL=http://localhost:5088` if the API is not served from the same origin.

## Senior Engineering Notes

- The backend uses ports (`IWorkforceReadModel`) so data storage can move from in-memory seed data to SQL, Cosmos DB, or a vendor system without changing controllers.
- The application layer owns use cases and response shapes. Controllers stay thin.
- Domain records are immutable where possible, which makes state easier to test and reason about.
- The frontend keeps API typing and fallback behavior in `services/`, not scattered across components.
- The UI is built as a working dashboard first, not a marketing page.


## CRUD engineering learning course

Follow the [workforce quote course](astraupskill/README.md) for server validation, honest API errors, stale response handling, complete test examples, exercises, and recorded verification.
