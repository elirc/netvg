# 25 Common Mid-Level Backend .NET/C# Interview Questions

## How to use this file

Use these as rehearsal answers, not scripts. A strong mid-level answer usually has three
parts:

1. A plain-English definition.
2. A practical example from backend work.
3. One tradeoff, failure mode, or production concern.

If an interviewer asks for depth, do not try to sound academic. Anchor the answer in
systems you have touched: ASP.NET APIs, C# services, databases, event consumers, tests,
React-facing API contracts, payroll/onboarding/compliance workflows, and production
debugging.

---

## 1. What is object-oriented programming?

**Short answer:** Object-oriented programming models software as objects that combine
data and behavior. The main ideas are encapsulation, abstraction, inheritance, and
polymorphism.

**Mid-level answer:** In C#, I use OOP mostly to model domain behavior and protect
invariants. For example, a `PayrollRun` or `Worker` should not just be a bag of public
properties if there are rules about valid states. Encapsulation lets the object expose
safe methods like `Approve()` or `MarkOnboardingComplete()` rather than letting every
caller mutate fields directly.

**Follow-up depth:** Inheritance is useful, but overused inheritance gets brittle. In most
business apps I prefer small interfaces, composition, and explicit domain methods over
deep class hierarchies.

---

## 2. Explain encapsulation, abstraction, inheritance, and polymorphism.

**Encapsulation:** Hide internal state and expose safe operations.

**Abstraction:** Expose what callers need without forcing them to know implementation
details. Example: application code depends on `IWorkforceReadModel`, not whether data
comes from EF Core, an API, or in-memory data.

**Inheritance:** Reuse or specialize behavior through a base class. Useful in limited
cases, but can create tight coupling.

**Polymorphism:** Different implementations can be used through the same interface or
base type. Example: `INotificationSender` could have email, SMS, and fake test
implementations.

**Mid-level framing:** The goal is not to use all four everywhere. The goal is code that
is easier to change safely.

---

## 3. What is the difference between an interface and an abstract class in C#?

**Short answer:** An interface defines a contract. An abstract class can define shared
state, constructors, and partial implementation.

**Mid-level answer:** I use interfaces at architectural boundaries, like repositories,
external clients, or domain services. They make substitution and testing easier. I use
abstract classes more cautiously, usually when a family of types truly shares behavior
and state.

**Tradeoff:** Too many interfaces can add ceremony if there is only one implementation
and no real boundary. But for infrastructure dependencies, interfaces are valuable because
they keep application code independent from EF, HTTP clients, or message brokers.

---

## 4. What is dependency injection and why does ASP.NET Core use it?

**Short answer:** Dependency injection means classes receive their dependencies from the
outside, usually through constructors, instead of creating them directly.

**Mid-level answer:** In ASP.NET Core, DI keeps controllers and services testable and
decoupled. A controller can depend on an application service, and the application service
can depend on an interface like `IHiringQuoteCalculator`. At runtime, the container wires
up the real implementation. In tests, I can provide a fake.

**Follow-up depth:** DI also manages lifetimes: singleton, scoped, and transient. A common
bug is a singleton depending on a scoped service like an EF `DbContext`, which can cause
stale state, threading problems, or disposed-object errors.

---

## 5. Explain singleton, scoped, and transient lifetimes.

**Singleton:** One instance for the whole application lifetime. Good for stateless,
thread-safe services or configuration.

**Scoped:** One instance per request in ASP.NET Core. This is common for EF `DbContext`
because one request usually represents one unit of work.

**Transient:** New instance every time it is requested. Good for lightweight stateless
objects.

**Mid-level answer:** The key is matching lifetime to state and thread safety. If a
service stores request-specific data, it should not be singleton. If it uses a scoped
dependency, it should usually be scoped too.

---

## 6. What is async/await in C# and why use it?

**Short answer:** `async`/`await` lets code perform asynchronous work without blocking a
thread while waiting for IO.

**Mid-level answer:** Backend APIs spend a lot of time waiting on databases, HTTP calls,
file storage, or message brokers. `await` releases the thread back to the thread pool
while the IO is in progress, which improves scalability under load. It does not make CPU
work faster; it helps with IO-bound work.

**Common pitfall:** Blocking on async with `.Result` or `.Wait()` can cause thread pool
starvation and deadlocks in some contexts. Prefer async all the way through.

---

## 7. What is a `CancellationToken` and where should it be used?

**Short answer:** A `CancellationToken` lets callers signal that work should stop.

**Mid-level answer:** In an ASP.NET API, if a client disconnects or a request times out,
the cancellation token can be passed down to EF Core, HTTP calls, and other async work.
That prevents the server from continuing expensive work nobody needs anymore.

**Production angle:** Cancellation is capacity management. It matters more when endpoints
call databases, queues, or external services. For in-memory demo code it may not matter,
but for real IO it should be passed through.

---

## 8. What is the difference between `IEnumerable`, `IQueryable`, and `List`?

**`IEnumerable<T>`:** A sequence you can iterate over. Execution may be deferred, but it
does not know how to translate expressions to a database.

**`IQueryable<T>`:** A query that can be composed and translated by a provider like EF
Core into SQL.

**`List<T>`:** A concrete in-memory collection.

**Mid-level answer:** With EF Core, filtering on `IQueryable` before materializing is
important because the database does the work. Calling `ToList()` too early pulls data into
memory and then filters in the app, which can become a performance problem.

---

## 9. What are value types and reference types in C#?

**Short answer:** Value types store the value directly. Reference types store a reference
to an object.

**Examples:** `int`, `decimal`, `bool`, `DateTime`, and structs are value types. Classes,
strings, arrays, and most objects are reference types.

**Mid-level answer:** The practical concern is mutation and nullability. Reference types
can be null unless nullable reference types are enabled and respected. Value types are
copied by value, although large structs can have performance concerns. For money, I
prefer `decimal` over `double` because decimal avoids binary floating-point rounding
surprises.

---

## 10. What is the difference between `record`, `class`, and `struct`?

**Class:** Reference type, commonly used for services, entities, and mutable objects.

**Record:** Usually used for immutable data shapes and value-based equality. Great for
DTOs, events, and simple domain values.

**Struct:** Value type. Useful for small immutable values, but should be used carefully.

**Mid-level answer:** Records are nice for request/response DTOs because they are concise
and compare by value. For domain entities with identity and lifecycle, a class may be a
better fit. I avoid large mutable structs because copying behavior can surprise people.

---

## 11. What are generics and why are they useful?

**Short answer:** Generics let you write type-safe code that works with different types.

**Example:** `List<T>`, `Task<T>`, `Result<T>`, and `IRepository<T>` are generic types.

**Mid-level answer:** Generics reduce duplication while preserving compile-time type
safety. `Task<DashboardDto>` tells callers exactly what async result to expect. A
`Result<T>` can consistently represent success/failure across application services
without returning `object` or throwing exceptions for expected validation outcomes.

---

## 12. How do exceptions work in C#? When should you throw vs return errors?

**Short answer:** Exceptions represent unexpected or exceptional failures. Expected
business outcomes are often better represented with validation results or typed results.

**Mid-level answer:** I throw for things like impossible states, infrastructure failures,
or bugs. I avoid throwing for normal validation like "missing required field" or "quote
input is invalid." For those, I return a result object or model validation response so the
API can return a useful `400 Bad Request`.

**Production angle:** Exceptions should be logged with context, but not leak sensitive
details to clients.

---

## 13. What is REST?

**Short answer:** REST is an architectural style for exposing resources over HTTP using
standard methods like GET, POST, PUT/PATCH, and DELETE.

**Mid-level answer:** A good REST API uses nouns for resources, meaningful status codes,
and predictable request/response contracts. For example, `GET /workers/{id}` reads a
worker, `POST /payroll-runs/{id}/approve` might trigger an approval command, and errors
should be structured enough for the frontend to handle.

**Tradeoff:** Not every business action maps perfectly to CRUD. It is fine to model
commands explicitly when the domain action matters, as long as the API is consistent and
documented.

---

## 14. What HTTP status codes should a backend engineer know?

**Common answers:**

- `200 OK`: request succeeded.
- `201 Created`: resource created.
- `202 Accepted`: request accepted for async processing.
- `204 No Content`: success with no response body.
- `400 Bad Request`: invalid input.
- `401 Unauthorized`: not authenticated.
- `403 Forbidden`: authenticated but not allowed.
- `404 Not Found`: resource does not exist or is not visible to the user.
- `409 Conflict`: request conflicts with current state.
- `429 Too Many Requests`: rate limited.
- `500 Internal Server Error`: unexpected server failure.

**Mid-level answer:** In event-driven systems, `202 Accepted` is useful when the request
starts async work but the read model may not update immediately.

---

## 15. What is middleware in ASP.NET Core?

**Short answer:** Middleware is code that runs in the HTTP request pipeline before or
after endpoints/controllers.

**Mid-level answer:** Middleware handles cross-cutting concerns like routing,
authentication, authorization, CORS, exception handling, HTTPS redirection, logging, and
security headers. Order matters. Authentication must happen before authorization. Global
exception handling should wrap downstream middleware.

**Follow-up depth:** Middleware is a good place for request-level concerns, but business
rules belong in application/domain services, not in middleware.

---

## 16. What is model validation in ASP.NET Core?

**Short answer:** Model validation checks incoming request DTOs against rules before or
during controller/action execution.

**Mid-level answer:** Basic validation can use data annotations or FluentValidation:
required fields, ranges, string lengths, date formats. Deeper domain validation belongs
in application/domain logic because it may require database state or business rules. The
API should return clear field-level errors where possible so the frontend can show useful
messages.

**Common mistake:** Duplicating all backend validation in the frontend. The frontend can
help the user, but the backend is authoritative.

---

## 17. What is Entity Framework Core?

**Short answer:** EF Core is Microsoft's object-relational mapper for .NET. It maps C#
objects and LINQ queries to relational database tables and SQL.

**Mid-level answer:** EF is useful because it handles change tracking, migrations, query
composition, and transactions. But you still need to understand SQL. Poor queries,
missing indexes, N+1 loading, and materializing too much data can hurt performance.

**Follow-up depth:** For read-heavy endpoints, project directly into DTOs with `Select`
instead of loading entire entity graphs. Use migrations intentionally and review generated
SQL for important paths.

---

## 18. What is the N+1 query problem?

**Short answer:** N+1 happens when the app makes one query to load a list and then one
additional query per item to load related data.

**Example:** Load 100 workers, then lazily load payroll info for each worker: 101 queries.

**Mid-level answer:** Fixes include eager loading with `Include`, explicit projection into
DTOs, batching, or denormalized read models. Projection is often best for API screens
because it fetches only the columns the UI needs.

**Production angle:** N+1 often hides in development because seed data is tiny. Measure
with SQL logs, query plans, and realistic data.

---

## 19. How do database indexes work and when would you add one?

**Short answer:** An index is a data structure that helps the database find rows faster,
usually at the cost of extra storage and slower writes.

**Mid-level answer:** I add indexes for columns frequently used in filters, joins, sorts,
or uniqueness checks. For example, a workforce dashboard might need indexes on `ClientId`,
`Status`, or `(ClientId, CountryCode)` depending on the query pattern.

**Tradeoff:** Indexes are not free. Every write must maintain them. I would check the
actual query plan and production-like query patterns rather than indexing every column.

---

## 20. What is a database transaction?

**Short answer:** A transaction groups operations so they either all succeed or all roll
back together.

**Mid-level answer:** Transactions protect consistency. If approving a payroll run writes
an approval record and updates status, those should usually commit together. In event
systems, the tricky part is coordinating database commits with message publishing. The
outbox pattern helps by writing the event to the same database transaction, then
publishing it asynchronously.

**Follow-up depth:** Know ACID: atomicity, consistency, isolation, durability. Also know
that distributed transactions are expensive and often avoided in microservice systems.

---

## 21. What is idempotency and why does it matter?

**Short answer:** An operation is idempotent if running it multiple times has the same
effect as running it once.

**Mid-level answer:** Idempotency matters for retries, double-clicks, network failures,
and message brokers with at-least-once delivery. If a payroll approval request is retried,
it should not approve twice, send duplicate notifications, or create duplicate records.

**Implementation examples:** Use idempotency keys, unique constraints, processed-message
tables, natural business keys, or state checks like "approve only if status is Pending."

---

## 22. What is event-driven architecture?

**Short answer:** Event-driven architecture uses events to communicate that something
happened, allowing other services to react asynchronously.

**Mid-level answer:** In an EOR/payroll platform, an event like `WorkerOnboarded` can
trigger payroll enrollment, benefits setup, billing, and notifications without the
onboarding service calling each system synchronously. This decouples teams and improves
resilience, but it introduces eventual consistency and harder debugging.

**Follow-up depth:** Consumers should be idempotent. Event schemas are contracts. Use
correlation IDs, dead-letter queues, retries, and schema/versioning rules.

---

## 23. What is the difference between authentication and authorization?

**Authentication:** Proving who the user is.

**Authorization:** Deciding what the authenticated user is allowed to do or see.

**Mid-level answer:** A user might be authenticated but not authorized to view a specific
client's workers or approve payroll. In APIs, authorization should be enforced server-side
using roles, claims, policies, tenant scoping, or resource-based checks. The frontend can
hide buttons, but it cannot be trusted as the security boundary.

---

## 24. What kinds of tests should a backend .NET service have?

**Short answer:** Unit tests, integration tests, and a smaller number of end-to-end or
contract tests.

**Mid-level answer:** Unit tests are good for domain rules, calculators, validators, and
pure services. Integration tests are good for controllers, EF queries, database behavior,
and message consumers. Contract tests help when a React app or another service depends on
an API/event schema.

**Testing instinct:** Test the riskiest behavior: money calculations, authorization,
idempotency, event replay, out-of-order events, and validation edge cases.

---

## 25. How would you design a payroll approval endpoint?

**Short answer:** Treat it as a business command, not just a button that flips a column.

**Mid-level answer:** I would add an authenticated endpoint like
`POST /payroll-runs/{id}/approve`. The API validates authorization, loads the payroll run,
checks it is in an approvable state, records who approved it and when, updates status in a
transaction, and publishes an event like `PayrollRunApproved` through an outbox. The
endpoint should be idempotent so a retry or double-click does not duplicate work.

**Operational concerns:** Add audit logging, structured logs with correlation IDs,
metrics around success/failure, clear `409 Conflict` responses for invalid state, and
tests for unauthorized users, already-approved runs, invalid IDs, and duplicate requests.

---

## Quick rehearsal plan

1. Practice questions 1-7 until the C# fundamentals feel automatic.
2. Practice questions 13-16 for ASP.NET API interviews.
3. Practice questions 17-21 for backend/data-heavy interviews.
4. Practice questions 22-25 for system design and production-readiness follow-ups.

