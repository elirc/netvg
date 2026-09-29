# 04 Interview Prep From This Repo

Use this repo to practice mid-level interviews while learning senior framing.

## C#/.NET Runtime Questions

**Question:** Why pass `CancellationToken` through async calls?  
**Codebase anchor:** Controller token at `src/PeblClone.Api/Controllers/WorkforceController.cs:14,25`; application passes it at `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:9-11`; adapter currently ignores it at `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:164-171`.  
**Junior answer:** It lets async calls be cancelled.  
**Mid-level adds:** The token should reach database/HTTP calls so abandoned requests stop doing work.  
**Senior includes:** Cancellation is part of reliability and capacity management; ignoring it is fine for in-memory seed data but risky for real IO.

**Question:** Why use records here?  
**Anchor:** `src/PeblClone.Domain/Workforce/Worker.cs:3-13`, `src/PeblClone.Application/Workforce/DashboardDtos.cs:5-68`.  
**Junior:** They are concise data classes.  
**Mid-level:** Good for immutable DTO/domain shapes.  
**Senior:** Positional records make constructor order part of the contract; add validation where invariants matter.

## ASP.NET Questions

**Question:** Explain the ASP.NET Core pipeline.  
**Anchor:** `src/PeblClone.Api/Program.cs:31-41`.  
**Junior:** Middleware runs before controllers.  
**Mid-level:** Order affects headers, CORS, HTTPS, health, and controller routing.  
**Senior:** Pipeline is an operational contract; auth, rate limiting, tracing, and exception handling should be deliberate.

**Question:** What is a DI lifetime bug?  
**Anchor:** scoped services at `src/PeblClone.Application/DependencyInjection.cs:8-12`; singleton read model at `src/PeblClone.Infrastructure/DependencyInjection.cs:9-12`.  
**Junior:** Services can be singleton/scoped/transient.  
**Mid-level:** Singleton depending on scoped service causes captive dependency.  
**Senior:** Current singleton is safe for static data; EF would force a scoped adapter.

## EF/Persistence Questions

**Question:** Where would EF Core fit?  
**Anchor:** application port `src/PeblClone.Application/Workforce/IWorkforceReadModel.cs:5-12`; current adapter `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:6-172`.  
**Junior:** In Infrastructure.  
**Mid-level:** Implement the port with DbContext and keep Application stable.  
**Senior:** Plan migrations, indexes, transaction boundaries, test data, rollback, and query performance.

## React Questions

**Question:** Explain render vs effect using this app.  
**Anchor:** effect at `src/PeblClone.Web/src/App.tsx:14-35`; render at `src/PeblClone.Web/src/App.tsx:50-232`.  
**Junior:** `useEffect` fetches data.  
**Mid-level:** Effects handle side effects; render uses state and props.  
**Senior:** Cleanup prevents late state updates; AbortController would also cancel work.

**Question:** What is stale client state risk here?  
**Anchor:** mock fallback at `src/PeblClone.Web/src/services/workforceApi.ts:29-34,43-77`.  
**Junior:** Mock data might be old.  
**Mid-level:** The UI can show data not from the API.  
**Senior:** In regulated/payroll contexts, silent fallback can become an incident; gate it and instrument failures.

## Debugging Questions

**Question:** Dashboard shows wrong attention count. How do you debug?  
**Anchor:** invariant `src/PeblClone.Domain/Workforce/Worker.cs:20-22`; summary `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:23`; UI `src/PeblClone.Web/src/App.tsx:100-103`.  
**Junior:** Check the UI value.  
**Mid-level:** Trace domain -> service -> DTO -> UI.  
**Senior:** Add regression coverage at the lowest layer that owns the invariant, then one integration test for the flow.

## System Design Questions

**Question:** Design payroll approval.  
**Anchor:** payroll record `src/PeblClone.Domain/Workforce/PayrollRun.cs:1-10`; display `src/PeblClone.Web/src/App.tsx:203-223`.  
**Junior:** Add a button and endpoint.  
**Mid-level:** Add command DTO, service, tests, UI state.  
**Senior:** Add authorization, idempotency, audit log, transaction, observability, rollback, and blast-radius control.

## Code Review Questions

**Question:** A PR duplicates quote pricing in the frontend. What do you say?  
**Anchor:** frontend duplicate `src/PeblClone.Web/src/services/workforceApi.ts:50-75`; backend source `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:36-65`.  
**Junior:** It works but is duplicated.  
**Mid-level:** Ask to centralize business rules.  
**Senior:** Identify contract drift risk, propose generated fixtures or disabling fallback in prod, and request tests.

## Behavioral Prompts

**Prompt:** Tell me about a time you reduced risk.  
**Repo story:** “I noticed unauthenticated workforce endpoints at `src/PeblClone.Api/Controllers/WorkforceController.cs:6-40`. I wrote an RFC for auth, scoped it to dashboard read access first, added API tests for 401/403, and staged rollout behind environment config.”

**Prompt:** Tell me about a tradeoff.  
**Repo story:** “The mock fallback at `src/PeblClone.Web/src/services/workforceApi.ts:29-34` improves demos but can hide outages. I kept it for development, added visible source state at `src/PeblClone.Web/src/App.tsx:69-72`, and proposed an env gate for production.”

