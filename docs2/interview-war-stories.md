# Interview War Stories — Velocity Global (Full-Stack SWE, ~1.5 years)

## How to use this document

These are **reconstructed story scaffolds**, not memories. They are built from three things
that are definitely true: you worked ~1.5 years at Velocity Global on their main web app
(an Employer of Record / global HR platform), the backend was **C#/.NET with an
event-driven architecture**, and the frontend was **React + TypeScript**.

Each story below is the *kind* of work that role guarantees you did. Your job before the
interview:

1. Read each story and see if anything sparks recognition — even a fragment ("yes, payroll
   deadlines were a thing" or "I remember the word 'supported employee'").
2. Replace bracketed `[placeholders]` with anything you can recover from: LinkedIn,
   old emails, Slack exports, your resume, former teammates, old Jira/GitHub notifications
   in your inbox.
3. Pick **5–6 stories max** and rehearse them out loud until they're yours. Interviewers
   probe for depth, so the technical follow-up notes under each story matter as much as
   the story itself.
4. Keep claims at mid-level scope: you owned features and fixed hard bugs; you did not
   single-handedly re-architect the platform.

**A note on honesty:** everything here is plausible for your real role, but don't invent
specific numbers (latency, revenue, headcount) you can't back up. Vague-but-true beats
precise-but-fabricated — "it noticeably cut support tickets" survives follow-up questions;
"it cut tickets 47%" invites one you can't answer.

---

## Domain refresher (so you can talk like you worked there)

Velocity Global is an **Employer of Record (EOR)**: companies use it to hire employees in
countries where they have no legal entity. Velocity Global is the legal employer; the
client company directs the day-to-day work. The platform handles:

- **Onboarding** — collecting worker documents, contracts, right-to-work checks per country.
- **Payroll** — monthly/semi-monthly runs across many countries and currencies, with hard
  statutory deadlines.
- **Compliance** — country-specific employment law: probation periods, notice periods,
  mandatory benefits, tax registrations.
- **Benefits & time off** — varies wildly per country.
- **Billing** — invoicing the client company (salary + employer costs + management fee).

Vocabulary that fits the domain: *supported employee*, *client*, *entity*, *payroll run /
payroll cycle*, *cutoff date*, *country playbook*, *quote / cost calculation*, *FX rates*.

Why event-driven fits this business (use this if asked "why events?"): onboarding and
payroll are **long-running, multi-step workflows** crossing team and system boundaries —
an employee signing a contract triggers payroll enrollment, benefits enrollment, billing
setup, and notifications, each owned by different services. Events decouple those so one
slow/failing consumer doesn't block the rest.

Plausible stack details to use carefully (say "we used a message broker — I believe it was
[RabbitMQ / Kafka / AWS SNS+SQS]" only if you can verify; otherwise just say "our message
bus"): .NET 6+, ASP.NET Core Web APIs, a message broker with publish/subscribe,
Entity Framework Core, SQL Server or PostgreSQL, React + TypeScript SPA, REST APIs to the
frontend, cloud-hosted (AWS or Azure), CI/CD with automated tests.

---

## Story 1 — The duplicate-event bug (debugging / "hardest bug")

**Answers:** "Tell me about the hardest bug you've fixed" / "a time you debugged something
tricky in production."

- **Situation:** Our services communicated through events. A downstream consumer I worked
  on handled `[an event like EmployeeOnboarded or PayrollRunApproved]`. We started seeing
  duplicate records — e.g., a worker appearing twice in a downstream view, or a
  notification firing twice.
- **Task:** I owned the investigation since it was my team's consumer.
- **Action:** I traced message IDs through the logs and found the broker was redelivering
  events when our handler took too long to ack — the handler did several slow operations
  (DB writes plus an external call) inside the message handling, occasionally exceeding the
  visibility/ack timeout, so the broker assumed failure and redelivered. The real lesson:
  **at-least-once delivery means consumers must be idempotent**, and ours wasn't. I fixed
  it by making the handler idempotent — keying processed message IDs in a dedupe table
  (checked in the same DB transaction as the write) — and moving the slow external call out
  of the critical path.
- **Result:** Duplicates stopped; we then audited our other consumers for the same pattern
  and made idempotency a checklist item in code review for new event handlers.

**Follow-up depth:** at-least-once vs at-most-once vs exactly-once (exactly-once is
effectively a myth across system boundaries; you get it via idempotency); dedupe table vs
natural idempotency (upsert by business key); why the dedupe check must share the
transaction with the write; out-of-order delivery as the sibling problem.

---

## Story 2 — Eventual consistency confusing users (full-stack / design tradeoff)

**Answers:** "Tell me about a tricky frontend/backend interaction" / "a time you improved UX."

- **Situation:** Because the backend was event-driven, some operations were asynchronous:
  the user submits something (e.g., approving `[a payroll run / an onboarding step]`), the
  API publishes an event and returns 202-style success, but the read model the React app
  displays doesn't update until consumers finish. Users would click approve, see no change,
  click again, and file support tickets ("the app is broken / I approved twice").
- **Task:** Make async processing feel coherent in the UI without faking it.
- **Action:** On the frontend I added an explicit **pending state** — the row immediately
  shows "Processing…" (optimistic status, not optimistic data) and the submit button
  disables. We polled the status endpoint with backoff until the read model caught up
  [or: listened on a websocket/SignalR channel, if you can verify that]. On the backend I
  made the approve endpoint idempotent so double-clicks were harmless regardless.
- **Result:** The double-submission tickets for that flow basically disappeared, and the
  pending-state pattern got reused on other async screens.

**Follow-up depth:** optimistic UI vs pending-state and when each is safe (don't show
optimistic *data* for money/compliance — show optimistic *status*); polling vs push
tradeoffs; CQRS read models and why the lag exists at all; idempotency keys on POST.

---

## Story 3 — The payroll cutoff incident (pressure / production incident)

**Answers:** "Tell me about working under pressure" / "a production incident you handled."

- **Situation:** Payroll has hard external deadlines — if a run misses a country's cutoff,
  real people get paid late. During `[a monthly payroll cycle]`, a batch of events stopped
  being processed: messages were piling up in the dead-letter queue because a consumer was
  throwing on a malformed/unexpected payload (a producer team had deployed a change to an
  event's schema — a renamed/now-null field our deserialization choked on).
- **Task:** It surfaced hours before a cutoff. I was on the team that owned the consumer
  and helped drive the fix.
- **Action:** Two tracks at once: short-term, we patched the consumer to tolerate the
  field change (defensive deserialization), deployed, then **replayed the dead-lettered
  messages** in order and verified counts matched. Longer-term, I was part of the
  follow-up: we treated event payloads as real contracts — versioned schemas, a rule that
  producers only make additive changes, and a contract test so a breaking event change
  fails the producer's CI instead of detonating in a consumer at month-end.
- **Result:** The run made the cutoff. The contract-testing follow-up mattered more than
  the patch — schema drift between teams stopped being a recurring failure mode.

**Follow-up depth:** dead-letter queues and replay strategy (idempotency makes replay
safe — ties to Story 1); schema evolution rules (additive-only, tolerant reader);
consumer-driven contract tests; alerting on DLQ depth so it's caught in minutes not hours.

---

## Story 4 — Disagreeing about where logic lives (conflict / influence)

**Answers:** "Tell me about a disagreement with a teammate" / "a time you changed your mind
or someone else's."

- **Situation:** Building `[a feature like an employment cost quote or a time-off accrual
  display]`, a teammate wanted to implement the calculation in the React app — faster to
  ship, instant feedback as the user changes inputs. I thought the business rules
  (country-specific employer costs, statutory rates) had to live server-side.
- **Task:** Resolve it without stalling the sprint.
- **Action:** Instead of arguing abstractly I wrote down the concrete failure cases:
  rates change mid-quarter and a stale SPA bundle shows wrong numbers; other consumers
  (billing, internal tools) would need the same logic and drift; we'd be exposing pricing
  internals in the client bundle. Then I proposed a middle path that kept the UX he cared
  about: the calculation stays in a .NET service, the frontend calls a lightweight quote
  endpoint with debounced input, and the API response includes the line-item breakdown so
  the UI stays rich.
- **Result:** We shipped server-side with nearly the same responsiveness. The thing I
  actually learned: leading with *his* requirement (instant feedback) rather than my
  objection is what ended the argument — it became a design problem, not a turf problem.
- **If asked for a time YOU were wrong, invert it:** you pushed for server-side rendering
  of something trivially cosmetic, a teammate showed the round-trip hurt UX with zero
  correctness benefit, and you conceded — rule of thumb you took away: *correctness logic
  on the server, presentation logic in the client.*

---

## Story 5 — Onboarding feature with ambiguous requirements (ownership / ambiguity)

**Answers:** "Tell me about a time requirements were unclear" / "a feature you owned
end-to-end."

- **Situation:** I picked up a feature on the onboarding flow: `[e.g., supporting a new
  document type / a new country's onboarding checklist / a contract-amendment flow]`. The
  ticket was one line. Onboarding steps differ per country, and the edge cases (what if
  the worker is mid-onboarding when rules change? what's required vs optional per country?)
  weren't specified anywhere.
- **Task:** I owned it across the stack — domain model, API, events, React UI.
- **Action:** Before writing code I wrote a short doc listing the edge cases as questions
  and got 30 minutes with `[the product manager / the ops team who actually onboard
  workers]` — the ops folks were the goldmine, they knew every weird case. Then I built it:
  the checklist definition data-driven per country rather than hardcoded (`if country ==
  "DE"` was the road to hell), an event published when a step completed so notifications
  and downstream tracking stayed decoupled, and a React checklist UI with clear
  required/optional/blocked states. I demoed to ops before release and caught one wrong
  assumption cheaply.
- **Result:** Shipped without rework; the data-driven structure meant the next country
  addition was config + tests, not code. Takeaway I cite: *an hour with the people who do
  the job manually is worth a week of guessing.*

**Follow-up depth:** how you modeled per-country variation (configuration/data over
branching code); domain events vs integration events; how you tested country variants
(parameterized tests).

---

## Story 6 — The slow dashboard (performance)

**Answers:** "Tell me about a performance problem you solved."

- **Situation:** A dashboard screen (e.g., a client's view of all their supported
  employees with statuses and upcoming payroll info) got slow as data grew — fine in dev,
  multi-second for large clients in production.
- **Task:** Make it fast without rewriting the feature.
- **Action:** Measured first instead of guessing. Two findings, one per layer. Backend:
  a classic **EF Core N+1** — loading workers, then lazily loading each one's related data
  per row; plus the endpoint returned everything and the frontend paginated in memory.
  Fixed with explicit projection (`Select` straight into a DTO, only the columns the screen
  needs), proper `Include`s where needed, server-side pagination, and an index on
  `[the common filter column]`. Frontend: the table re-rendered every row on each keystroke
  in the filter box — fixed with debounced filtering and memoized row components
  (`React.memo` / `useMemo`).
- **Result:** Large-client load went from seconds to comfortably sub-second, and "measure
  before optimizing" became my reflex — both fixes were invisible in dev-sized data.

**Follow-up depth:** how you measured (SQL logging / query plans, React DevTools
profiler); projection vs `Include` vs lazy loading; when you'd reach for a denormalized
read model instead (ties to the event-driven CQRS angle); pagination contract changes
(API now takes page/size, returns total count).

---

## Story 7 — A mistake that taught you something (failure / accountability)

**Answers:** "Tell me about a time you failed / made a mistake."

- **Situation:** I shipped a change to an event consumer that worked perfectly in testing
  but assumed events arrive **in order**. In production under load, two related events
  occasionally arrived swapped (e.g., an "updated" event before its "created" event, or
  consumers on parallel workers), and we got `[a small number of records in a bad state]`.
- **Task:** Own the cleanup and make sure it couldn't recur.
- **Action:** Flagged it as soon as we spotted it rather than quietly fixing — wrote up
  what happened, queried the blast radius, wrote a script to repair the affected records,
  then fixed the real flaw: the handler became order-tolerant (treat an update for an
  unseen entity by deferring/retrying, or upsert with version checks) instead of assuming
  the happy path. Added a test that replays the events shuffled.
- **Result:** Small actual impact, fixed within a day. The durable lesson — distributed
  systems guarantee less than your local test environment implies; ordering, like
  exactly-once, is something you design for, not assume — and I started reviewing other
  people's handlers for the same assumption.

**Why this story works:** the mistake is genuinely subtle (seniors make it), you
self-reported, you fixed data + cause + class-of-bug. That's exactly the failure-story
shape interviewers want.

---

## Story 8 — TypeScript migration / taming a messy frontend (initiative / tech debt)

**Answers:** "Tell me about improving code quality" / "initiative beyond your tickets."

- **Situation:** Parts of the React codebase had drifted: API response types declared as
  `any` or hand-written interfaces that no longer matched what the .NET API actually
  returned. We kept hitting runtime errors that TypeScript should have caught — a renamed
  backend property silently becoming `undefined` in the UI.
- **Task:** Self-initiated, scoped small enough to do alongside feature work.
- **Action:** Proposed generating the TypeScript types from the API's OpenAPI/Swagger
  spec instead of maintaining them by hand `[e.g., with NSwag or openapi-typescript]`,
  so backend contract changes surfaced as frontend **compile** errors. Did it
  incrementally — wired the generation, migrated the modules my team owned as we touched
  them, wrote a short how-to for other teams; no big-bang rewrite.
- **Result:** That class of silent breakage went away for migrated modules and the
  pattern spread to other teams. Good story because it connects the C# and TS halves of
  the stack: *the API contract is one source of truth and both ends should derive from it.*

---

## Story 9 — Cross-team collaboration on an event contract (collaboration)

**Answers:** "Tell me about working with another team" / "influencing without authority."

- **Situation:** My team needed new data in an event another team produced (e.g.,
  billing needed `[a field like contract effective date or employer cost breakdown]` on an
  onboarding event). Their backlog was full; the tempting shortcut was calling their
  database or internal API directly.
- **Task:** Get what we needed without creating coupling we'd regret.
- **Action:** Pushed back on the direct-DB shortcut (it bypasses their ownership and
  breaks silently when they refactor). Instead I wrote the proposed event change myself —
  additive field, schema notes, and the consumer-side handling — and brought it to them as
  a small reviewable PR plus a 15-minute conversation, rather than a ticket in their queue.
  They reviewed and shipped it in a fraction of the time.
- **Result:** Got the data through the proper contract; "bring the other team a PR, not a
  request" became my default for cross-team asks.

---

## Quick-fire answers (non-STAR questions)

**"Walk me through the architecture you worked in."**
"An employer-of-record platform — clients hire employees in countries where they have no
entity, and the platform runs onboarding, payroll, compliance, and billing. Backend was
C#/.NET services communicating through events on a message bus, because the core flows are
long multi-step workflows crossing service boundaries — a signed contract kicks off payroll
enrollment, benefits, billing, notifications, each owned separately. Synchronous REST APIs
fed the React/TypeScript frontend, with read models updated by event consumers. I worked
full-stack: domain logic and event consumers in .NET, and the React screens on top."

**"What did you like / dislike about event-driven architecture?"**
Like: decoupling (teams ship independently), resilience (a slow consumer doesn't block the
publisher), natural audit trail, replay-ability. Dislike: debugging is harder — a request
becomes a story scattered across services, so correlation IDs and good tracing are
oxygen; eventual consistency leaks into UX (Story 2); and event schemas are API contracts
that are *easier* to break accidentally than REST ones (Story 3).

**"Why did you leave / gap in employment?"**
Honest and brief is fine: "I was in an accident and took time off to recover. I'm fully
ready to get back to work, and I've been sharpening my .NET and React skills in the
meantime." You are not obligated to mention memory loss; "recovery from an accident" is
true and sufficient. Practice saying it in two sentences without apologizing.

**"What's your experience with testing?"**
xUnit/NUnit unit tests on domain logic and calculators (money/compliance math is the
high-value target); integration tests on event consumers — including the unhappy paths:
duplicate delivery, out-of-order delivery, malformed payloads (Stories 1, 3, 7); React
side, component tests with React Testing Library; types generated from the API spec as a
cheap contract test (Story 8).

---

## Things to verify before the interview (fill these in)

| Question | Where to look | Answer |
|---|---|---|
| Exact job title & dates | LinkedIn, resume, offer letter email | |
| Team name / product area (payroll? onboarding? billing?) | Old emails, Slack, former teammates | |
| Message broker (Kafka? RabbitMQ? AWS SNS/SQS? Azure Service Bus?) | Teammates; job postings for VG often list the stack | |
| Cloud provider (AWS/Azure) | Same | |
| Database (SQL Server? PostgreSQL?) | Same | |
| .NET version era (you were there ~2023–2024 → likely .NET 6/7/8) | Resume bullets | |
| One real teammate/manager name for references | LinkedIn connections | |
| Any real project names or resume bullets to anchor stories to | Resume, old performance review emails | |

**Tip:** look up current Velocity Global engineering job postings — companies list their
real stack in them (this is the fastest way to confirm broker/cloud/DB and say the names
confidently).

## Rehearsal plan

1. Pick 5–6 stories: one bug (1), one incident (3), one conflict (4), one ownership (5),
   one failure (7), plus 6 or 8 depending on whether the role leans backend or frontend.
2. Say each out loud in under 2 minutes. Record yourself once.
3. For each, prepare for the two follow-ups an interviewer will ask: "what exactly did
   *you* do?" and "what would you do differently?"
4. One story can answer many prompts — practice re-leading the same story for "pressure,"
   "debugging," and "collaboration."
