# Full-Stack & Miscellaneous Interview Q&A

## How to use this file

Companion to `interview-backend-dotnet-qa.md` (25 backend .NET/C# questions) and
the three war-stories files. This file covers everything *else* a full-stack
interview loop hits, mapped to your resume skills: JavaScript/TypeScript, React,
Angular, Node, SQL, AWS, API design, AI/LLM tooling, and the behavioral/HR
questions that aren't STAR stories.

Same answer shape as the backend file:

1. Plain-English definition.
2. Practical example anchored in work you've done.
3. One tradeoff, failure mode, or production concern.

War-story cross-references point at `interview-war-stories.md` (1–9),
`interview-war-stories2-more.md` (10–20), and
`interview-war-stories3-contracting.md` (21–32).

---

# Part 1 — JavaScript & TypeScript

## 1. What's the difference between `var`, `let`, and `const`?

**Short answer:** `var` is function-scoped and hoisted with `undefined`; `let` and
`const` are block-scoped; `const` prevents reassignment (not mutation).

**Mid-level answer:** In practice I write `const` by default, `let` only when
reassignment is genuinely needed, and `var` never. `const` doesn't freeze objects —
`const user = {...}` still allows `user.name = x` — which trips people up. The
block-scoping difference matters in loops with closures: `var i` in a loop shares
one binding across all iterations' callbacks; `let i` gives each iteration its own.

---

## 2. Explain the event loop and async/await.

**Short answer:** JavaScript runs on one thread with an event loop: synchronous
code runs to completion, then queued callbacks (microtasks like resolved promises
first, then macrotasks like timers) run. `async/await` is syntax over promises.

**Mid-level answer:** The practical consequence: nothing else runs while your
synchronous code runs. In a Node API, CPU-heavy work in a handler blocks *every*
request, not just that one — I/O is where Node shines because it's delegated and
non-blocking. `await` pauses the function, not the thread; other requests keep
being served.

**Failure mode I check for in review:** sequential `await`s for independent
operations — `await a(); await b();` when `Promise.all([a(), b()])` would halve
the latency. Also unhandled rejections from forgetting to await.

---

## 3. What is a closure and where does it bite you?

**Short answer:** A function that captures variables from its defining scope, which
stay alive as long as the function does.

**Mid-level answer:** Closures are the bread and butter of callbacks, hooks, and
module patterns. Where they bite: **stale captures**. The classic React version is
a `useEffect` or event handler closing over an old state value — the handler "sees"
the value from the render it was created in, not the current one. The fix is
dependency arrays, functional updates (`setCount(c => c + 1)`), or refs, depending
on the case.

---

## 4. Why TypeScript? When has it actually saved you?

**Short answer:** Types move a class of runtime bugs to compile time and make code
self-documenting and refactorable.

**Mid-level answer:** The concrete win from my experience: keeping frontend types
in sync with backend contracts. A renamed property in a C# DTO silently becomes
`undefined` in plain JS — with typed (ideally generated) API models it becomes a
compile error in the frontend before anyone runs anything. (War story 8 — types
generated from the OpenAPI spec.)

**Tradeoff:** `any` is a virus — one `any` at an API boundary silently de-types
everything downstream. I prefer `unknown` + narrowing at boundaries, and types
generated from a source of truth over hand-maintained ones, which rot.

---

## 5. `==` vs `===`, `null` vs `undefined` — quick hits

- `===` compares without coercion; use it always. The one idiomatic exception some
  codebases allow: `x == null` to match both `null` and `undefined`.
- `undefined` = never assigned; `null` = deliberately empty. In API contracts I
  care more about the practical question: does the backend distinguish "field
  omitted" from "field set to null"? (PATCH semantics — it often matters.)
- Optional chaining `?.` and nullish coalescing `??` cleaned up most of this;
  remember `??` differs from `||` for `0` and `""` — a real bug source in forms.

---

# Part 2 — React

## 6. Walk me through how you structure a React app.

**Mid-level answer:** Component-driven, one-way data flow. Presentational
components take props and emit callbacks — no fetching, no business rules. Data
fetching and mutations live in a thin API layer `[hooks / React Query-style]`;
shared state goes only as high as it must. Feature folders over type folders
(`features/quotes/` containing its components, hooks, and types) so a feature is
deletable. Server state and UI state are different problems — caching/refetching
libraries for the former, local state or context for the latter. (War story 23 is
the narrative version; this repo's `src/PeblClone.Web` follows the same shape.)

---

## 7. When does a component re-render, and how do you fix excessive re-renders?

**Short answer:** When its state changes or its parent re-renders. Memoization
(`React.memo`, `useMemo`, `useCallback`) cuts re-renders, but measure first.

**Mid-level answer:** My real-world example: a table re-rendering every row on
every keystroke in a filter input (war story 6). Fixes in order: debounce the
input, memoize row components, keep the input's state local so typing doesn't
re-render the table at all. The ordering matters — restructuring state placement
often beats sprinkling `useMemo` everywhere.

**Tradeoff:** premature memoization adds noise and its own bugs (stale closures in
`useCallback` deps). Profile with React DevTools before optimizing.

---

## 8. `useEffect` — what is it for, and what are the common mistakes?

**Short answer:** Synchronizing the component with something outside React —
subscriptions, timers, manual DOM, fetches (though dedicated data libraries do
fetching better).

**Common mistakes I look for in review:**
- Missing/wrong dependency arrays → stale data or infinite loops.
- No cleanup → leaked subscriptions, or state updates on unmounted components.
- Race conditions: two fetches in flight, the older one resolves last and wins. Fix
  with an aborted/ignored flag in cleanup or `AbortController`.
- Using an effect for derived state that should just be computed during render.

---

## 9. How do you handle forms and validation in React?

**Mid-level answer:** Layered validation (war story 14): the client handles
immediate, cheap checks — required fields, formats, disabled-while-submitting —
for fast feedback; the server stays authoritative for business rules and returns
**field-level** errors that I map back onto the form rather than one generic
banner. Preserve the user's input on failure; wiping a form is hostile. Controlled
components for anything that needs live validation; don't reimplement every
backend rule in TypeScript — that's duplicated logic that *will* drift.

---

# Part 3 — Angular (and the comparison question)

## 10. How does Angular differ from React, practically?

**Mid-level answer:** Angular is a framework — DI, router, HTTP client, forms, and
RxJS are built in and opinionated; React is a library you assemble a stack around.
Angular uses classes, decorators, and templates with services injected via DI;
React uses functions, hooks, and JSX. Change detection vs reconciliation differ
under the hood but the daily-work difference is: Angular makes more decisions for
you (good for large teams and consistency), React gives more freedom (good for
velocity, easier hiring).

**The answer that lands in interviews:** "I've shipped both professionally, and
the architecture I aim for is identical — dumb components, one-way data flow, a
single API seam. The frameworks just spell it differently: services + observables
vs hooks + props." (War story 23.)

## 11. Quick Angular hits to be ready for

- **Services & DI:** singleton services via `providedIn: 'root'`; components stay
  thin, logic and HTTP live in services. Mirrors constructor injection in .NET —
  say that; interviewers like the cross-stack connection.
- **RxJS:** observables are lazy, cancellable, multi-value streams vs promises'
  single eager value. Know `subscribe` leak hygiene (`async` pipe in templates
  beats manual subscriptions), and `switchMap` for "cancel the previous request"
  (typeahead — the same race condition as the `useEffect` fetch race in Q8).
- **Reactive vs template-driven forms:** reactive for anything non-trivial —
  testable, composable validators, dynamic fields.

---

# Part 4 — Node & API design

## 12. Where does Node fit, and what are its limits?

**Short answer:** Great for I/O-bound services — APIs that orchestrate databases
and third-party calls — because of the non-blocking model and the shared language
with the frontend. Weak for CPU-bound work, which blocks the event loop.

**Mid-level answer:** At SourcedCode I built/maintained Node APIs backing Angular
and React frontends. The shared-language benefit is real (shared types, no mental
switching); the discipline Node demands is keeping handlers I/O-only and pushing
heavy computation elsewhere `[worker threads / a queue / a separate service]`. I
also bring .NET habits into Node: layering, validation at the boundary, and not
letting "it's JavaScript" mean "it's unstructured."

## 13. How do you design a REST API?

**Mid-level checklist answer:** Resources and nouns (`/workers/{id}/payroll-runs`),
correct verbs and status codes (and 422 vs 400 vs 409 distinctions where useful),
pagination from day one on anything that grows (war story 19), field-level
validation errors, consistent error envelope, versioning strategy decided before
you need it (additive changes preferred), idempotency for anything money- or
workflow-shaped — idempotency keys on POST, naturally idempotent PUT.

**The depth move:** mention that the contract is the real product — generated
client types (war story 8) and the rule that breaking changes fail CI, not
production (war story 3's contract tests).

## 14. How do you handle authentication and authorization?

**Short answer:** Authentication = who you are (sessions, JWTs, OAuth2/OIDC);
authorization = what you may do (roles, claims, resource-level checks). Never
trust the client for either.

**Mid-level answer:** Patterns I've implemented: JWT bearer tokens validated by
middleware, OAuth2 client-credentials for service-to-service and third-party
integrations (war story 22), API keys for simpler integrations — stored hashed,
scoped, rotatable. The bug class I specifically watch for: **missing resource-level
authorization** — the user is authenticated and has a role, but nothing checks the
requested record belongs to their tenant (war story 11). The UI choosing what to
request is convenience; the API deciding what's allowed is security.

## 15. How do you make an integration with a third-party API reliable?

**Mid-level answer:** Assume it fails. One typed client wrapper owning auth and
headers; an explicit mapping layer so their model doesn't leak into ours (their
schema change breaks one seam, loudly); timeouts always; retries with backoff +
jitter on transient failures only — and only when the call is idempotent; their
request IDs logged for cross-side tracing; defensive parsing so an unexpected null
degrades instead of crashing. Webhooks over polling when offered, but verify
signatures and handle redelivery (idempotency again). (War story 22.)

---

# Part 5 — SQL & data

## 16. How do you find and fix a slow query?

**Mid-level answer:** Execution plan first, guesses never. The usual suspects in
the order I find them: missing index on a join/filter column; non-sargable
predicates (function-wrapped columns defeating indexes); correlated subqueries
doing per-row work that should be set-based; `SELECT *` hauling unused columns;
the N+1 pattern when an ORM is involved. After fixing, prove result equivalence —
a fast wrong report is worse than a slow right one. (War stories 6 and 31.)

**Depth:** indexes aren't free — write cost and maintenance; sometimes the answer
is a pre-aggregated or denormalized read model rather than heroic query tuning.

## 17. Quick SQL hits

- **JOIN types:** INNER (matches only), LEFT (keep left side, NULLs for misses) —
  and know that a `WHERE` on the right table's column silently turns a LEFT JOIN
  into an INNER. Real bug source in reports.
- **Index intuition:** an index is a sorted lookup structure; it helps filters,
  joins, and sorts on its leading columns. Composite index column order matters.
- **Transactions:** ACID one-liner, then the practical bit — keep transactions
  short, and know that the dedupe-check-plus-write in war story 1 had to share a
  transaction to be correct.
- **NULL:** three-valued logic — `NULL = NULL` is not true; `NOT IN` with NULLs
  returns nothing and ruins someone's afternoon.

---

# Part 6 — AWS & operations

## 18. What's your AWS experience?

**Honest scoped answer:** "Practical application-level use, not infrastructure
engineering: Lambda for scheduled automation and small pipelines, S3 for storage
and delivery, `[RDS / SES / CloudWatch — only what you can verify]`. I've owned
small serverless systems end to end including the operational parts — alerting on
failure, secrets management, idempotent runs." (War story 30.)

**Depth if pushed:** Lambda's limits (timeout, package size, cold starts) and when
they make a container/server the right call; least-privilege IAM as the default
posture; why scheduled jobs must be safe to run twice (and detectably failed when
run zero times).

## 19. How do you debug something in production you can't reproduce locally?

**Mid-level answer:** Widen observability first, then correlate: structured logs
around the suspect path, a correlation ID to follow one request/workflow across
services (war story 13), and a timestamp from the user to anchor the search.
Form a hypothesis from the pattern (input-shaped? load-shaped? timing-shaped?),
then try to encode it as a failing test before fixing (war story 26). The
contractor version of this skill — no docs, no one to ask — is the strongest proof
I have of it.

---

# Part 7 — AI / LLM questions (expect these)

## 20. How do you use AI tools in your development workflow?

**The strong answer:** "Daily and deliberately. Agentic tools — Claude Code, Codex
— handle the mechanical layer: boilerplate from an exemplar, test scaffolding,
code archaeology in unfamiliar repos. The judgment layer stays mine: design,
review, edge cases, anything touching auth, money, or migrations. I review
generated code like a junior engineer's PR. Net effect: initial development time
on routine changes dropped from hours to minutes, and my hours moved to the work
that actually needs an engineer." (War stories 24–26 are the evidence.)

## 21. What are the failure modes of AI-generated code?

**Answer from experience, not theory:** Plausible-looking wrongness is the whole
category. Specific flavors I've caught: tests "fixed" by weakening assertions;
APIs that don't exist in the project's library version; errors silently swallowed
to make the happy path demo well; generic-tutorial patterns that ignore project
conventions; subtle security gaps (missing tenant scoping — the AI doesn't know
your authorization model). The guardrails: agents work on branches, diffs read
line-by-line with extra attention to test changes, sensitive areas re-derived by
hand. (War story 25.)

## 22. "Doesn't AI make developers like you less necessary?"

**Don't be defensive; reframe:** "It changed the job's center of gravity. Typing
was never the bottleneck — knowing what to build, verifying correctness, and
owning the result were. AI compresses the typing and inflates the value of review
and judgment, because there's *more* plausible code to evaluate, faster. The
engineers at risk are the ones whose whole value was the typing. My last three
years have been building exactly the other skill set: specifying, verifying, and
taking responsibility for AI-accelerated work."

## 23. Have you built anything *with* LLMs (not just using them)?

`[Fill from truth — even small counts: AI-assisted boilerplate pipelines with
refined per-project prompts (war story 24) are legitimately "building a workflow
on LLMs." If you've called the APIs directly — completions, structured outputs,
embeddings — say so concretely. If not, say "my depth is in AI-assisted
engineering workflows rather than LLM product features, and the API layer is
something I can pick up fast" — honest beats bluffed here, the follow-ups are
easy to fail.]`

---

# Part 8 — Behavioral & HR (non-STAR)

## 24. "Tell me about yourself." (the 90-second version)

Rehearse this cold — it sets the tone for everything:

"I'm a full-stack engineer, about `[6-7]` years in, mostly C#/.NET and
TypeScript/React with Angular and Node in the mix. I started freelance — building
sites, automation, and reporting tools for small businesses, which taught me to
own delivery end to end. Then about a year and a half at Velocity Global, an
employer-of-record platform, doing event-driven backend services in .NET and React
frontends at real scale. Since 2023 I've been contracting across several small
companies — full-stack feature work, third-party integrations, and I've gone deep
on AI-assisted development: agentic tools for boilerplate, archaeology, and tests,
with the review discipline to keep quality up. I'm looking for `[depth on one
product / a team to grow with — your real answer]`."

## 25. "What's your greatest weakness?"

Pick something true, non-fatal, with a visible mitigation. Good fits for your
profile:

- "Contracting trained me to bias toward shipping independently — on a team I have
  to consciously surface work-in-progress earlier rather than appearing with a
  finished thing. I've built the habit of early draft PRs and design notes."
- "I over-invest in polishing things past the point of diminishing returns; I now
  timebox and ask 'what does done mean' up front."

Avoid humble-brags ("I work too hard") and genuinely disqualifying answers.

## 26. "Why do you want to work here?" / "Why this role?"

Build it fresh per company from three parts: something specific about their
product/domain (read their engineering blog and job post), the honest career
reason (depth after breadth — see war story file 3's quick-fire), and a concrete
match ("you're React + .NET — that's exactly my center of gravity").

## 27. "Where do you see yourself in five years?"

Senior IC trajectory is a safe, honest frame: deeper ownership of systems,
mentoring, and being the person who makes the team's AI-leverage responsible and
effective — that last one is differentiating and true to your resume.

## 28. "Why did you leave Velocity Global?" / employment-gap questions

Two sentences, no apology, then forward-looking: "I was in an accident and took
time to recover. I'm fully ready to work, and I've kept my .NET and React skills
sharp in the meantime." (Same guidance as war-stories file 1 — practice until it's
boring.)

## 29. "Do you have questions for us?" — always yes. Good ones:

- "What does the first 90 days look like for this role — what would 'going well'
  mean?"
- "How does the team use AI tooling today, and what's the appetite for more?"
  (Plays directly to your strength.)
- "What's the messiest part of the codebase or process right now?" (Signals you
  expect reality, not a brochure.)
- "How do changes get from idea to production — who decides what, and how long
  does it take?"
- For startups: "What's the runway/funding picture?" — legitimate and expected.

## 30. Salary expectations

Don't name a number first if avoidable: "I'd want to understand the full role and
package — can you share the budgeted range?" Most jurisdictions/companies now post
or share ranges. If forced, give a researched range for your market and level, not
a point. Decide your walk-away number *before* the call, not during it.

---

## Coverage map — where everything lives

| Topic | File |
|---|---|
| Backend .NET/C# concepts (OOP, DI, async, EF, etc.) | `interview-backend-dotnet-qa.md` |
| Velocity Global STAR stories | `interview-war-stories.md`, `interview-war-stories2-more.md` |
| Contracting / freelance STAR stories, AI stories | `interview-war-stories3-contracting.md` |
| JS/TS, React, Angular, Node, SQL, AWS, AI Q&A, behavioral | this file |

## Rehearsal priorities for this file

1. Q24 (tell me about yourself) — out loud, until it's 90 seconds and boring.
2. Q20–22 (AI questions) — these *will* come up and they're your differentiator.
3. Q10 (Angular vs React) — guaranteed question given the resume.
4. Q13–15 (API design, auth, integrations) — the technical core of contractor work.
5. Q28 (gap) — two calm sentences.
