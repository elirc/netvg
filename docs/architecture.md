# Architecture Notes

AtlasWork is intentionally small, but it is organized like a system that can grow.

## Backend

The backend follows a layered architecture:

- `Domain` contains enterprise concepts: workers, countries, compliance checks, payroll runs, and money.
- `Application` contains use cases and ports. It decides what a dashboard means and how a hiring quote is calculated.
- `Infrastructure` adapts external details into application ports. Today that is deterministic seed data; tomorrow it could be SQL, a payroll vendor, or an HRIS API.
- `Api` is the composition root. It owns HTTP routing, CORS, health checks, OpenAPI, and service registration.

The important senior habit is dependency direction: outer layers can depend on inner layers, but inner layers do not know about HTTP, React, databases, or vendors.

## Frontend

The React app starts with the operational dashboard because that is the main product experience. API contracts live in `services/`, shared formatting lives in `features/formatters.ts`, and reusable display primitives live in `components/`.

The dashboard uses a mock fallback when the API is unavailable. That is useful for demos and local teaching, but the fallback is deliberately isolated so production can disable or replace it later.

## Production Next Steps

- Replace `InMemoryWorkforceReadModel` with a database-backed adapter.
- Add authentication and tenant-aware authorization.
- Add audit logging around quote creation, payroll approval, and compliance changes.
- Add contract tests between the API and frontend.
- Add observability with structured logs, metrics, traces, and dashboard alerts.
