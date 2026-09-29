# Interview War Stories 2 - More Velocity Global Story Scaffolds

## How to use this companion file

This file continues `interview-war-stories.md`. These are still **reconstructed
story scaffolds**, not guaranteed memories. Use them the same way:

1. Keep only the ones that spark a real memory or match a resume bullet.
2. Replace bracketed placeholders with details you can verify.
3. Do not memorize all of them. Pick a small set and rehearse until they sound natural.
4. Keep the claims mid-level: "I owned the implementation / investigation / follow-up,"
   not "I saved the entire platform."

The first file already covers the strongest core set: duplicate events, eventual
consistency, payroll cutoff pressure, conflict, ambiguous requirements, performance,
failure/accountability, TypeScript contracts, and cross-team event collaboration.

The stories below fill in other common interview prompts: security, rollout strategy,
data correctness, observability, accessibility, mentoring, refactoring, and production
support.

---

## Story 10 - Currency rounding in a payroll or quote calculation (correctness / detail)

**Answers:** "Tell me about a bug where details mattered" / "a time you handled money or
high-correctness logic."

- **Situation:** We had a calculation flow for `[employment cost quotes / payroll totals /
  client invoices]` where amounts crossed currencies and country-specific fees. A small
  mismatch appeared between the UI total and the backend/exported total.
- **Task:** I needed to find the source without treating it as "just a display bug,"
  because payroll and billing numbers have to be explainable.
- **Action:** I traced the same sample worker through each layer: raw inputs, backend
  calculation, API DTO, React formatting, and exported value. The problem was not one big
  formula error; it was inconsistent rounding. One layer rounded each line item before
  summing, another summed raw decimals and rounded only the final total. I moved the rule
  into the .NET calculation service, made the API return both raw-ish calculation fields
  and display-ready totals, and added tests around known edge cases like zero-decimal
  currencies, exchange rates, and negative adjustments.
- **Result:** The UI, API, and export matched. The broader lesson was that money logic
  needs one source of truth and tests with boring-looking edge cases, because that is
  where real incidents hide.

**Follow-up depth:** decimal vs floating point; rounding per line vs final total; storing
minor units vs decimals; why formatting belongs in the UI but calculation rules belong on
the server; using golden test cases from finance/ops.

---

## Story 11 - Client data scoping bug caught before release (security / privacy)

**Answers:** "Tell me about reducing risk" / "a security-minded decision you made."

- **Situation:** I was working on a dashboard/API endpoint that returned `[workers /
  onboarding tasks / payroll summaries]` for a client. During testing, we noticed the
  query filtered by status and country but did not consistently filter by the current
  client's tenant/account ID in one path.
- **Task:** Fix the immediate risk and make sure similar endpoints were not exposed to the
  same mistake.
- **Action:** I added the missing server-side scope filter and wrote a regression test
  with two clients that had similar workers so the bug could not hide behind tiny seed
  data. I also pushed for the authorization context to be part of the service method
  contract instead of something each query remembered manually. On the frontend, I kept
  client IDs out of trusted decision-making; the UI can choose what to request, but the
  API decides what the user is allowed to see.
- **Result:** We caught it before release, and the tests became a pattern for other
  client-scoped read endpoints. It is a good interview story because it shows privacy
  judgment without needing to claim there was an actual breach.

**Follow-up depth:** tenant isolation; authorization vs authentication; never trusting
client-provided IDs; test fixtures with multiple tenants; defense in depth at query,
service, and API layers.

---

## Story 12 - Feature flag rollout for a country-specific flow (release strategy)

**Answers:** "Tell me about a rollout" / "a time you managed risk while shipping."

- **Situation:** We were adding or changing a country-specific flow like `[new onboarding
  document requirements / benefits enrollment / payroll cutoff behavior]`. The tricky
  part was not the UI alone; the rules affected ops workflows and downstream events.
- **Task:** Ship the change without surprising all users or blocking ongoing onboarding
  cases already in progress.
- **Action:** I put the new behavior behind a feature flag/config switch scoped by
  country and, where needed, by client. Existing in-flight workers stayed on the old rule
  set unless ops explicitly migrated them. I added logging around which path was used and
  created a quick rollback plan: flip the flag, stop publishing the new event shape, and
  leave existing records readable. We released to `[one country / one pilot client]`
  first, watched support feedback and logs, then expanded.
- **Result:** The change shipped with a clean escape hatch. The important framing: feature
  flags are not just for product experiments; in compliance-heavy systems they are a way
  to control blast radius.

**Follow-up depth:** config-driven country rules; backward compatibility for in-flight
records; feature flags vs branches; rollback planning; observability during rollout.

---

## Story 13 - Correlation IDs across async workflows (observability / debugging)

**Answers:** "How do you debug distributed systems?" / "a time you improved production
support."

- **Situation:** Debugging an onboarding or payroll issue meant jumping between the
  original API request, an event published by a .NET service, one or more consumers, and
  the React user report. Logs existed, but a single user action was hard to follow across
  services.
- **Task:** Make investigation faster without requiring a new platform rewrite.
- **Action:** I helped standardize correlation IDs. The API accepted or generated a
  request/correlation ID, logged it with structured properties, and included it as event
  metadata when publishing messages. Consumers copied the ID into their own logs and into
  any follow-up events they emitted. I also updated a runbook with the exact queries to
  follow one workflow from API call to event consumer to final read-model update.
- **Result:** Production investigations became less guessy. Instead of searching by email,
  timestamp, and luck, we could trace one workflow end to end and know where it stopped.

**Follow-up depth:** structured logging vs plain text logs; correlation ID vs causation
ID; propagation through message headers; log sampling; why user PII should not be the
primary debugging key.

---

## Story 14 - Making a complex form harder to submit wrong (frontend UX / quality)

**Answers:** "Tell me about improving a user workflow" / "a frontend problem you solved."

- **Situation:** A React form for `[onboarding documents / worker profile details /
  contract information]` had a lot of country-specific fields. Users could get to the end
  and only then find out something was invalid, or they could save data that passed
  frontend checks but failed server validation.
- **Task:** Make the form guide the user without duplicating all backend business logic in
  TypeScript.
- **Action:** I separated validation into layers. The frontend handled immediate checks:
  required fields, date format, obvious constraints, disabled submit while saving, and
  inline error placement. The backend remained authoritative for country/compliance
  rules and returned field-level validation errors instead of one generic failure. I
  mapped those server errors back to the relevant fields and kept unsaved user input in
  place so a failed submit did not wipe work.
- **Result:** The form felt less punishing, support got clearer screenshots/error states,
  and we avoided the trap of reimplementing every compliance rule twice.

**Follow-up depth:** client validation vs server validation; accessibility of error
messages; preserving dirty form state; field-level API errors; why disabled buttons need
visible reasons.

---

## Story 15 - Replaying events safely after a production fix (operations / reliability)

**Answers:** "Tell me about production support" / "how do you recover from bad data?"

- **Situation:** A consumer bug caused a set of `[onboarding / payroll / billing]` events
  to fail and land in a dead-letter queue. After fixing the code, we still had to process
  the missed messages.
- **Task:** Replay the messages without creating duplicates or corrupting already-fixed
  records.
- **Action:** I treated replay as a controlled operation, not a button click. First I
  identified the exact message range and affected business IDs. Then I verified the
  consumer was idempotent, replayed a small batch in lower environment or with a dry-run
  query where possible, and monitored counts before and after. For the production replay,
  we ran in batches, checked error rate and downstream totals, and kept a rollback/repair
  script ready.
- **Result:** The missed workflow caught up cleanly. The story pairs well with duplicate
  events: replay is only safe when handlers are designed for at-least-once delivery.

**Follow-up depth:** DLQ triage; replay order; idempotency keys; dry runs; metrics to
watch during replay; when a repair script is safer than replay.

---

## Story 16 - Refactoring a brittle service without changing behavior (maintainability)

**Answers:** "Tell me about improving legacy code" / "a refactor you are proud of."

- **Situation:** A service around `[country rules / quote calculation / onboarding status]`
  had grown through many tickets. It worked, but every new country or exception added
  another conditional and made regressions more likely.
- **Task:** Improve the structure while keeping the business behavior stable.
- **Action:** I started by adding characterization tests around the existing behavior:
  representative countries, edge cases ops cared about, and one or two historical bugs.
  Then I refactored in small steps: extracted named policy/calculation objects, moved
  repeated date/money logic into helpers already used by the domain, and kept the public
  API contract unchanged. Each step was reviewable and behavior-preserving.
- **Result:** The next feature in that area was easier to add and easier to test. The
  interview point is restraint: the refactor was not "make it pretty"; it reduced the
  risk and cost of future country-specific changes.

**Follow-up depth:** characterization tests; refactor vs rewrite; branch-by-abstraction;
keeping contracts stable; knowing when not to refactor during a deadline.

---

## Story 17 - Mentoring through code review (leadership / collaboration)

**Answers:** "Tell me about mentoring someone" / "how do you give feedback?"

- **Situation:** A newer teammate opened a PR for a full-stack feature. The code worked,
  but business logic was split between React and .NET, tests only covered the happy path,
  and some naming did not match domain language.
- **Task:** Help them improve the PR without turning review into a wall of criticism.
- **Action:** I grouped feedback by risk. Blocking comments were about correctness:
  centralize the rule server-side, add tests for country variants, and make the endpoint
  idempotent. Non-blocking comments were phrased as suggestions: naming, small component
  cleanup, and follow-up refactor ideas. I also paired for 30 minutes on the first test so
  they were not left guessing what I meant.
- **Result:** The PR shipped stronger, and future PRs from that teammate came in with
  better tests and clearer domain language. The lesson: good review teaches the system's
  values, not just the reviewer's preferences.

**Follow-up depth:** blocking vs non-blocking feedback; pairing after review; reviewing
for risk first; domain language in code; avoiding nitpicks that bury important issues.

---

## Story 18 - Handling stale data in a React table (frontend state / async systems)

**Answers:** "Tell me about a tricky frontend state bug" / "a time async data caused a
problem."

- **Situation:** A table showed `[workers / payroll runs / onboarding tasks]` and allowed
  filtering or actions like approve, cancel, or mark complete. Users sometimes saw stale
  rows after an action because the backend processed asynchronously and the frontend cache
  assumed the mutation was immediately reflected in the read model.
- **Task:** Make the UI honest about the system state.
- **Action:** I changed the flow so the action put the row into a local pending state with
  a timestamp and disabled only the affected controls, not the whole page. We refetched
  after the mutation and again after a short delay/backoff until the read model reflected
  the change. If the backend reported failure, the row returned to its previous state and
  showed a specific error. I also made sure filter/sort state survived the refresh so the
  page did not jump around.
- **Result:** Users got clear feedback, double-actions dropped, and the UI stopped
  implying a synchronous guarantee the backend did not provide.

**Follow-up depth:** local pending state; invalidating queries; preserving filter state;
optimistic UI vs honest async UI; why regulated workflows need explicit failure handling.

---

## Story 19 - API pagination added after data growth (scalability / API design)

**Answers:** "Tell me about scaling a feature" / "a backend/frontend contract change."

- **Situation:** An endpoint originally returned all `[workers / tasks / payroll items]`
  because early clients had small datasets. As real clients grew, the response became slow
  and the React table did too much work in memory.
- **Task:** Add server-side pagination and filtering without breaking existing consumers.
- **Action:** I introduced a paged endpoint/contract with `page`, `pageSize`, filter, and
  sort parameters, plus `totalCount` in the response. On the backend I projected directly
  into DTOs and applied filtering before materializing the query. On the frontend I moved
  pagination controls to use the API state, added loading/empty/error states, and kept the
  old endpoint temporarily or versioned the change depending on consumer impact.
- **Result:** Large datasets became usable, and the endpoint contract matched the UI's
  actual needs instead of leaking early assumptions.

**Follow-up depth:** offset vs cursor pagination; total counts can be expensive; sorting
stability; backward-compatible API changes; measuring payload size and query time.

---

## Story 20 - Turning a support pattern into a product fix (product thinking)

**Answers:** "Tell me about taking initiative" / "a time you used customer/support
feedback."

- **Situation:** Support kept seeing questions around `[why onboarding was blocked / why
  payroll approval was pending / what document was missing]`. The system had the data, but
  the UI made users infer the reason from several fields.
- **Task:** Reduce confusion without building a huge new workflow.
- **Action:** I reviewed a handful of support tickets with product/ops and grouped them by
  root cause. The common pattern was missing context, not missing capability. I added a
  clear status reason field from the backend and surfaced it in the React UI with concise
  copy like "Waiting on signed contract" or "Pending payroll cutoff review." I also made
  sure the status reason came from controlled values instead of free-text strings so it
  could be translated, tested, and tracked.
- **Result:** The workflow became more self-explanatory. The story shows product sense:
  sometimes the best feature is making the existing state legible.

**Follow-up depth:** controlled status enums; backend-owned workflow state; UX copy for
  operational tools; measuring support ticket themes; avoiding unstructured status text.

---

## Story selection map

Use these when you need variety beyond the first file:

| Prompt | Best story |
|---|---|
| Security / privacy | Story 11 |
| Release strategy | Story 12 |
| Distributed debugging | Story 13 |
| Frontend UX | Story 14 or 18 |
| Production recovery | Story 15 |
| Refactoring | Story 16 |
| Mentoring | Story 17 |
| API scalability | Story 19 |
| Product sense | Story 20 |

## Strongest combined interview set

If you only rehearse six total stories across both files, use:

1. Story 1 - duplicate-event bug.
2. Story 3 - payroll cutoff incident.
3. Story 4 - disagreement about where logic lives.
4. Story 5 - ambiguous onboarding ownership.
5. Story 7 - failure/accountability.
6. One of Story 11, 13, 16, or 17 depending on the role.

