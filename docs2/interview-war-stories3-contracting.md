# Interview War Stories 3 — SourcedCode Contracting & Upwork Freelance

## How to use this file

This file continues `interview-war-stories.md` (stories 1–9) and
`interview-war-stories2-more.md` (stories 10–20), which cover Velocity Global.

These stories cover the **other two eras of your resume**:

- **SourcedCode (Feb 2023 – Present)** — contractor across several small clients:
  Oye Hearing, Watts & Associates, Hypernova, PIC. Resume-verified facts: agentic
  coding with Claude Code and Codex; C#/.NET and JavaScript API integrations with
  authentication, mapping, and error handling; Angular and React component-driven
  frontends on Node backend APIs and third-party services; AI-automated boilerplate
  for APIs, components, and tests.
- **Upwork (Apr 2019 – Aug 2021)** — freelance: wireframes to responsive/accessible
  pages; automation and reporting tools in JavaScript, Python, SQL, AWS; SQL for
  CRUD, reporting, and analysis.

Same rules as the earlier files:

1. These are **reconstructed scaffolds**, not memories. Keep the ones that spark
   recognition or match something you can verify (old repos, invoices, Upwork job
   history, client emails).
2. Replace `[placeholders]` with real details. Your Upwork profile's job history
   and feedback are a goldmine — every completed contract is a dated, titled record
   of what you actually built.
3. Don't invent metrics. "It saved the client a recurring manual task every week"
   survives follow-ups; "it saved 14 hours a week" invites one you can't answer.
4. Contractor stories have a built-in strength: **breadth, autonomy, and client
   communication**. Lean on those. The honest scope is "I was the developer the
   client trusted to figure it out," not "I led a team."

**Why these stories matter in 2026 interviews:** the SourcedCode bullets are your
answer to the question every interviewer now asks — *"how do you work with AI
tools?"* Stories 24–26 are written specifically for that. Rehearse at least one.

---

## Story 21 — Ramping onto an unfamiliar client codebase fast (learning / adaptability)

**Answers:** "Tell me about learning something quickly" / "how do you get productive
in a new codebase?"

- **Situation:** As a contractor through SourcedCode, every engagement started the
  same way: a codebase I'd never seen, written by people who were often no longer
  around, and a client paying by the hour who wanted progress, not a two-week
  reading period. At `[Oye Hearing / Watts & Associates / Hypernova / PIC]` the
  project was `[an Angular app on a Node API / a .NET service / a React frontend]`.
- **Task:** Be safely productive — shipping real changes without breaking things I
  didn't know existed — within days, not weeks.
- **Action:** I built a repeatable ramp-up routine. First, run the app and trace one
  real user flow end to end before reading anything else — UI to API to database.
  Second, use agentic tools (Claude Code) to accelerate the mapping: ask it to
  summarize the module structure, find where a given feature's logic lives, and
  explain unfamiliar patterns — then **verify its answers against the code** rather
  than trusting them, because the verification is where the actual learning happens.
  Third, make my first change something small and observable (a bug fix or copy
  change) to validate my mental model of the build, test, and deploy loop before
  touching anything load-bearing.
- **Result:** I went from "new repo" to merged changes in the first few days on each
  engagement, and the routine itself became something I could describe to clients
  up front — which set realistic expectations and built trust early.

**Follow-up depth:** how you decide what *not* to read; tracing one flow vs reading
top-down; using AI for code archaeology while verifying claims; why your first
commit should be deliberately boring.

---

## Story 22 — Integrating a client system with an external API (integration / reliability)

**Answers:** "Tell me about an integration you built" / "a time you dealt with an
unreliable third party."

- **Situation:** A client needed their system connected to an external service —
  `[a scheduling/CRM platform for the hearing clinic / a data provider for the
  consulting work / a payment or messaging API]`. The external API had its own
  auth scheme, its own data model that didn't match the client's, and the usual
  third-party realities: rate limits, occasional timeouts, and fields that were
  sometimes null when the docs said they wouldn't be.
- **Task:** Build the integration in `[C#/.NET / Node]` so the client's data stayed
  reliable even when the third party wasn't.
- **Action:** Three layers. **Auth:** implemented `[OAuth2 client-credentials / API
  key / token refresh]` handling in one place — a typed client wrapper — so no other
  code dealt with tokens or headers. **Mapping:** wrote an explicit translation
  layer between their model and ours instead of letting their field names leak
  through the codebase; that one seam absorbed every subsequent change they made.
  **Error handling:** treated the third party as guaranteed-to-fail-sometimes —
  timeouts and retries with backoff on transient errors, no retries on 4xx, logging
  the request ID from their responses so support issues could be traced on both
  sides, and defensive parsing so an unexpected null degraded gracefully instead of
  crashing the sync.
- **Result:** The integration ran without babysitting. When the vendor changed a
  payload months later, the failure was caught at the mapping seam with a clear log
  message instead of as corrupted data downstream — which is exactly what the
  design was for.

**Follow-up depth:** retry policy details (idempotent vs non-idempotent calls,
backoff with jitter); circuit breaking; anti-corruption layer as the name for the
mapping seam; webhook vs polling tradeoffs; how you tested against an API you don't
control (recorded responses, sandbox accounts, contract assumptions as tests).

---

## Story 23 — Component-driven frontend across Angular and React (frontend / architecture)

**Answers:** "Angular vs React?" / "how do you structure a frontend?" / "tell me
about a frontend you built."

- **Situation:** Across SourcedCode clients I worked in **both** Angular and React —
  whichever the client already had. `[One client]`'s app had grown as pages that
  each did everything: data fetching, business rules, and markup tangled together,
  with the same UI patterns re-implemented slightly differently on every screen.
- **Task:** Build new features without multiplying the inconsistency, and leave the
  codebase more maintainable than I found it.
- **Action:** I pushed the code toward component-driven architecture incrementally:
  small presentational components that take data in and emit events out, with data
  fetching and business state lifted to `[container components and services in
  Angular / hooks and a thin API layer in React]`. New screens were composed from
  the shared pieces; old screens got migrated only when a ticket already touched
  them — no big-bang rewrite on a client's budget. The framework-agnostic insight I
  lead with in interviews: **the architecture is the same in both** — one-way data
  flow, dumb components, a single seam where the API is called — Angular and React
  just spell it differently (services/RxJS vs hooks/props).
- **Result:** New features got faster to build because they were assembly rather
  than invention, and visual inconsistencies stopped appearing because there was
  one source of truth per UI pattern. Working in both frameworks also made me
  better at each — you learn what's essential vs what's framework habit.

**Follow-up depth:** smart/dumb component split; where state should live (and why
"as low as possible, as high as necessary"); RxJS observables vs hooks mental
models; when a shared component is premature; how you handled forms in each
framework (reactive forms vs controlled components).

---

## Story 24 — Building an AI boilerplate pipeline that the team actually trusted (AI / leverage)

**Answers:** "How do you use AI in your workflow?" / "tell me about improving team
productivity."

- **Situation:** At SourcedCode the projects were small-team, deadline-driven, and
  full of repetitive structure: every new entity needed an API endpoint, DTOs,
  validation, a frontend component, and tests — all following existing patterns in
  the codebase. Writing the fifth one by hand teaches you nothing and burns client
  hours.
- **Task:** Automate the boilerplate with AI agents without lowering the quality
  bar — the failure mode being plausible-looking generated code that subtly ignores
  the project's conventions.
- **Action:** The key move was **showing the agent the pattern, not describing
  it**: point Claude Code or Codex at an existing exemplar — "here is our worker
  endpoint, its DTOs, its tests; create the equivalent for appointments" — so the
  output inherited the project's real conventions instead of generic tutorial code.
  I kept the human effort where it belongs: I reviewed generated code with the same
  rigor as a junior developer's PR, and the generated **tests** had to fail when I
  deliberately broke the code — that's how you know they test something. Over time
  I refined reusable prompts per project that encoded the conventions explicitly.
- **Result:** The mechanical first 80% of a feature went from hours to minutes, and
  the time went into the part clients actually pay for — the business logic and the
  edge cases. The framing I use in interviews: AI didn't replace the engineering
  judgment, it **moved all of my time onto the work that requires it**.

**Follow-up depth:** how you validate generated tests (mutation-style spot checks);
where AI output fails most (cross-cutting concerns, security, project-specific
invariants); prompt-as-documentation; why "show an exemplar" beats "describe the
style"; what you'd never delegate to an agent (auth, money math, migrations)
without line-by-line review.

---

## Story 25 — Catching AI-generated code that was confidently wrong (judgment / accountability)

**Answers:** "Tell me about a time AI got it wrong" / "how do you verify AI output?"
/ "what are the limits of AI coding tools?"

- **Situation:** Using agentic coding daily means you accumulate real examples of
  the failure modes. The one I cite: an agent produced a change that **looked
  exactly right** — clean code, plausible naming, passing tests — but `[handled an
  edge case wrong: e.g., it "fixed" a failing test by weakening the assertion, or
  used an API method that didn't exist in the project's library version, or
  silently swallowed an error to make the happy path work]`.
- **Task:** Catch it before the client's users did, and adjust my workflow so the
  same class of slip couldn't get through again.
- **Action:** I caught it in review because of a rule I'd already adopted: **the
  more confident and complete AI output looks, the more deliberately I read it** —
  plausibility is exactly what these models optimize for, so "looks right" carries
  no information. After that incident I formalized the guardrails: agents work on a
  branch, never directly on anything deployable; diffs get read line-by-line with
  special attention to test changes (a weakened test is the subtlest failure mode);
  and anything touching auth, money, or data migrations gets re-derived by hand
  even if the generated version looks fine.
- **Result:** Nothing AI-generated has shipped a defect under my name, which is the
  only metric that matters — the client doesn't care who typed the bug. The
  interview framing: AI raised my output a lot and my **responsibility** not at
  all; the review skill became more important, not less.

**Why this story works:** every interviewer in 2026 wants to know whether you're an
AI-leveraged engineer or an AI-dependent one. A specific, owned failure-and-guardrail
story answers that better than any claim.

---

## Story 26 — Debugging an undocumented legacy system for a client (debugging / autonomy)

**Answers:** "Hardest bug at your current job" / "tell me about working with legacy
code" / "a time you had no one to ask."

- **Situation:** A client reported that `[a recurring job / a form submission / a
  report]` had started failing intermittently. The catch: the system was built by a
  previous developer, there was no documentation, no tests around the failing area,
  and **no one to ask** — as the contractor, I *was* the escalation path.
- **Task:** Find and fix it without a safety net, on a system I couldn't fully see.
- **Action:** I treated reproduction as the whole battle. First I added structured
  logging around the failing path and got the client to tell me the next time it
  happened, so I had a timestamp to correlate. The pattern that emerged:
  `[failures clustered around specific input data / a timeout under load / a race
  between two operations]`. I used Claude Code to accelerate the archaeology —
  tracing where a value originated across an unfamiliar codebase in minutes instead
  of hours — but the hypothesis testing was mine: I wrote a failing test that
  reproduced the issue *before* fixing it, so the fix was provable rather than
  vibes. Then I fixed the root cause and left the logging and the test in place as
  the documentation that hadn't existed.
- **Result:** The intermittent failure stopped, and the client's system was more
  diagnosable than before I touched it. My contractor rule: **leave every system
  more observable than you found it**, because next time the clock starts on the
  logs you wrote last time.

**Follow-up depth:** strategies when you can't reproduce locally; binary-search
debugging; characterization tests on legacy code; how much logging is too much;
when to recommend the client invest in fixing the area vs patching.

---

## Story 27 — Scoping a feature with a non-technical client (requirements / communication)

**Answers:** "Tell me about gathering requirements" / "working with non-technical
stakeholders" / "a time you prevented wasted work."

- **Situation:** Small-company clients don't have product managers. At
  `[Oye Hearing / PIC]`, the request arrived as a sentence — "we need `[the system
  to send reminders / a dashboard for X / customers to be able to do Y]`" — from
  someone who knew their business deeply and software not at all.
- **Task:** Turn that sentence into something buildable without weeks of back-and-
  forth, and without building the wrong thing confidently.
- **Action:** My approach is to **show, not ask**. Instead of sending a list of
  abstract questions, I'd build the cheapest possible concrete artifact — a
  mock-up, a clickable prototype, or even AI-generated throwaway scaffolding of the
  screen — and review *that* with the client. People who can't answer "what should
  the edge-case behavior be?" can instantly say "no, that's wrong, it should work
  like this" when looking at a screen. I also wrote a one-page summary after each
  conversation — what we agreed, what's in, what's explicitly out — which served as
  lightweight scope protection for both sides.
- **Result:** The features I shipped matched what the client actually meant, not
  what they first said, and the written summaries repeatedly settled "I thought it
  would also do X" conversations amicably. That habit — concrete artifacts plus
  written confirmation — is most of what people mean by "good with clients."

**Follow-up depth:** prototypes vs specs; how AI changed prototyping economics
(throwaway scaffolding is nearly free now); scope creep handling on hourly vs fixed
contracts; how you flag "that's a bigger change than it sounds" without alarming
the client.

---

## Story 28 — Juggling multiple clients and priorities (prioritization / professionalism)

**Answers:** "How do you prioritize?" / "tell me about managing competing demands"
/ "describe your work style."

- **Situation:** As a contractor I regularly carried `[two or more]` active clients
  at once — different stacks (Angular for one, React/.NET for another), different
  expectations, and the contractor reality that everyone considers their thing the
  priority.
- **Task:** Keep every client feeling like a priority without lying to anyone about
  timelines.
- **Action:** Two mechanisms. **Batching:** context switching is the silent killer,
  so I gave clients contiguous blocks (days or half-days) instead of interleaving
  hours, and used a written end-of-block note — what I did, what's next, what I'm
  waiting on — so re-entering a project days later cost minutes, not an afternoon.
  **Honest sequencing:** when two things genuinely collided, I told the affected
  client immediately with a specific new date rather than going quiet and hoping —
  "quiet and hoping" is how contractors lose clients. Urgent production issues
  jumped the queue by an explicit, stated rule that all clients knew.
- **Result:** Clients renewed and referred, which for a contractor is the entire
  scoreboard. The transferable claim for full-time roles: I'm practiced at managing
  my own queue, communicating slippage early, and switching stacks without a ramp —
  because for three years that *was* the job.

---

## Story 29 — Wireframe to responsive, accessible pages (Upwork / frontend craft)

**Answers:** "Tell me about your attention to detail" / "experience with
accessibility?" / "an early project that shaped how you work."

- **Situation:** On Upwork I took contracts turning designs and wireframes into
  working pages — `[a marketing site / a small business site / an internal tool]`.
  One client's existing site was the classic failure pile: unusable on phones,
  div-soup markup, forms you couldn't navigate by keyboard.
- **Task:** Deliver pages that matched the design *and* worked for real users —
  phones, keyboards, screen readers — not just the designer's monitor.
- **Action:** Built mobile-first with semantic HTML as the foundation: real
  `<button>` and `<label>` elements, heading hierarchy, landmarks — because correct
  semantics deliver most of accessibility for free, and bolted-on ARIA can't rescue
  bad markup. Layout with flexbox/grid rather than pixel-pushing, tested at real
  breakpoints and with keyboard-only navigation. Where the design itself had
  problems — `[contrast too low / touch targets too small]` — I flagged it to the
  client with the reason rather than silently shipping an inaccessible design.
- **Result:** The client got pages that worked everywhere, and I got habits that
  compounded: I still write semantic-first markup in React and Angular today, and
  accessibility findings I raise now come from instinct built doing this work by
  hand.

**Follow-up depth:** semantic HTML before ARIA; how you test (keyboard pass,
Lighthouse/axe, screen reader spot check); responsive units and breakpoint
strategy; why accessibility is cheapest at build time.

---

## Story 30 — Automation that replaced a manual reporting process (Upwork / impact)

**Answers:** "Tell me about a project with measurable impact" / "experience with
Python/AWS?" / "a time you automated something."

- **Situation:** An Upwork client ran a recurring manual process: someone
  `[exported data from one system, massaged it in Excel, and emailed a report]`
  every `[week / day]`. Hours of skilled-person time spent on copy-paste, with
  occasional human errors that were worse than the time cost.
- **Task:** Automate it end to end, cheaply enough to make sense for a small
  business.
- **Action:** Built a pipeline with `[Python / Node]`: pull from the source
  `[via API / SQL query / file export]`, transform and validate, generate the
  report, deliver it `[by email / to a dashboard / to S3]`. Ran it on
  `[AWS Lambda on a schedule / a cron job]` — serverless because a small client
  shouldn't pay for an idle server. The part juniors skip and I didn't: **failure
  handling**. The job alerted on failure instead of silently not running, validated
  the source data before transforming (the manual human had been silently catching
  bad rows — the automation had to do the same), and logged enough to debug a bad
  run remotely.
- **Result:** A recurring multi-hour manual task became a scheduled job the client
  stopped thinking about, and the validation actually caught source-data errors the
  manual process had been missing. My favorite version of automation: not just
  faster, but *more correct* than the human process it replaced.

**Follow-up depth:** idempotency for scheduled jobs (what if it runs twice / not at
all); Lambda limits and when they bite; secrets management for client credentials;
monitoring something that runs while nobody watches.

---

## Story 31 — Rescuing a slow SQL report (Upwork / data)

**Answers:** "Tell me about SQL optimization" / "a database problem you solved."

- **Situation:** A client's `[report / dashboard / nightly job]` had slowed to
  minutes as their data grew — fine at launch, unusable at real scale. The query
  underneath had grown the same way the business had: incrementally, with nobody
  ever looking at the whole.
- **Task:** Make it fast without changing what it reported.
- **Action:** Started with the execution plan, not guesses. Findings were the
  classics: `[a function applied to a column in the WHERE clause killing index use
  / no index on the join column / a correlated subquery running per row / SELECT *
  feeding columns nobody used]`. Fixed in order of impact: added the right index
  (after checking write-side cost), rewrote `[the correlated subquery as a join /
  the per-row logic as a set operation]`, and selected only needed columns. Then I
  **proved equivalence** — old and new queries against the same data, results
  diffed — because a fast wrong report is worse than a slow right one.
- **Result:** Minutes to `[seconds / sub-second]`, same numbers. The durable skill:
  reading execution plans and thinking in sets instead of rows, which is the same
  instinct that catches EF Core and ORM-generated query problems in my .NET work
  today.

**Follow-up depth:** reading an execution plan (scans vs seeks); index design and
why not to index everything; sargability; when to denormalize or pre-aggregate
instead of optimizing further; how this maps onto ORM-generated SQL.

---

## Story 32 — A freelance project that went sideways (failure / boundaries)

**Answers:** "Tell me about a project that didn't go well" / "a difficult client" /
"what did freelancing teach you?"

- **Situation:** Early in the Upwork period I took a contract scoped as
  `[a "small" site update / a "quick" data tool]`. I priced and planned it off the
  client's description without verifying anything — and the reality underneath
  (`[a hacked-together codebase / data far messier than described / requirements
  that grew every call]`) bore little resemblance to the description.
- **Task:** Deliver something honest from a hole I'd dug myself, and stop digging
  it on future contracts.
- **Action:** Once it was clear the original scope was fiction, I named it directly
  rather than grinding silently into resentment: showed the client specifically
  what was different from what was described, re-scoped to a smaller deliverable
  that solved their most important problem, and ate some of the overrun as the cost
  of my own bad estimate. Afterward I changed how I started every engagement: a
  short paid discovery step — look at the actual code and data *before* committing
  to scope — and written scope with explicit exclusions.
- **Result:** That client relationship survived because the bad news came early and
  honestly. The lesson I carry into team settings: estimates made on unverified
  assumptions aren't estimates, they're wishes — and surfacing a slip immediately
  is recoverable, while surfacing it at the deadline never is.

**Why this story works:** failure stories from freelancing show maturity that's
hard to demonstrate any other way — you owned the estimate, the client
relationship, and the financial consequence personally.

---

## Quick-fire answers (contracting-era questions)

**"Walk me through what you do at SourcedCode."**
"I'm a contractor working across several small-company clients — a hearing-care
business, a consulting firm, a couple of others. The work is full-stack and varies
by client: C#/.NET and Node on the backend, Angular or React on the frontend,
integrations with third-party APIs. The constant is that I'm usually the developer
responsible for the whole change — scoping it with a non-technical stakeholder,
building it across the stack, and supporting it after. The last couple of years
I've also built AI-assisted workflows into how I deliver — agentic tools for
boilerplate, code archaeology, and test generation — which changed where my hours
go: less typing, more design and review."

**"Why contracting, and why a full-time role now?"** *(adjust to your real intent)*
Contracting after Velocity Global gave breadth: multiple stacks, direct client
ownership, and the discipline of being the only escalation path. The honest pitch
for full-time: "I've proven I can operate autonomously; what I want now is depth —
one product, one team, problems too big for one person."

**"Which do you prefer, Angular or React?"**
Don't pick a religion. "I'm productive in both and I've shipped both
professionally. They converge on the same architecture — one-way data flow,
presentational components, a thin API seam. React is lighter and the ecosystem
moves faster; Angular's structure is genuinely useful on bigger teams because the
framework makes more decisions for you. I'd use whichever the team already has —
consistency beats preference."

**"How much of your code does AI write?"**
The honest, strong answer: "A lot of the first draft, almost none of the final
judgment. Boilerplate, scaffolding, test skeletons, and code archaeology are
heavily delegated; design decisions, review, edge cases, and anything touching
auth or money are mine. I review generated code like a junior engineer's PR —
the throughput gain is real, but so is the new failure mode of plausible-looking
wrong code, and the discipline is what separates leverage from liability."
(Then offer Story 25.)

**"What was your AWS experience?"**
Keep it scoped to truth: "Practical, not platform-engineering — Lambda for
scheduled automation, S3 for storage and delivery, `[RDS / SES / whatever you can
verify]`. I've built and operated small serverless pipelines end to end, including
the unglamorous parts: alerting, secrets, and what happens when a job fails at 3am
with nobody watching."

---

## Story selection map (this file)

| Prompt | Best story |
|---|---|
| Learning quickly / new codebase | Story 21 |
| Integration / third-party APIs | Story 22 |
| Frontend architecture / Angular vs React | Story 23 |
| AI workflow / productivity | Story 24 |
| AI limits / judgment | Story 25 |
| Hard bug / legacy code / autonomy | Story 26 |
| Requirements / stakeholders | Story 27 |
| Prioritization / work style | Story 28 |
| Accessibility / frontend craft | Story 29 |
| Automation / Python / AWS | Story 30 |
| SQL / data | Story 31 |
| Failure / difficult client | Story 32 |

## Combined rehearsal set across all three files

A balanced six covering all eras and prompt types:

1. **Story 1** (VG) — duplicate-event bug → "hardest bug."
2. **Story 3** (VG) — payroll cutoff incident → "pressure / incident."
3. **Story 4** (VG) — where logic lives → "conflict."
4. **Story 24 or 25** (SourcedCode) — AI workflow / AI judgment → the 2026 question.
5. **Story 22 or 27** (SourcedCode) — integration or client scoping → "current work."
6. **Story 32** (Upwork) — project gone sideways → "failure," with personal stakes.

This set shows range deliberately: big-company event-driven systems at Velocity
Global, autonomous multi-stack delivery as a contractor, and ground-floor craft
from freelancing.
