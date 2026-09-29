# Proposed Improvements

Not bugs — design and quality changes that would make the codebase stronger, especially given its teaching intent. Ordered roughly by value.

---

## P-1 — Decouple application tests from Infrastructure seed data

**Where:** `tests/PeblClone.Application.Tests/*.cs`

Both test classes instantiate `InMemoryWorkforceReadModel` directly and assert against its seed (`TotalWorkers.Should().Be(5)`, `TotalMonthlyCost.Should().Be(12_799m)`). Consequences:

- The Application test project references Infrastructure, inverting the dependency story the README sells ("the application layer owns use cases").
- Editing demo seed data — adding a sixth worker, tweaking Germany's lead time — breaks unit tests that have nothing to do with the change.
- The `12_799m` assertion hard-codes the pricing constants a second time, so a legitimate fee change requires editing tests in lockstep with no independent check.

**Proposal:** Give the tests a small stub `IWorkforceReadModel` with purpose-built fixtures per test (2–3 workers constructed inline). Keep one thin integration test that exercises the real seed if you want coverage of it. This is also the perfect place to demonstrate why the port exists — right now the port's testability benefit is claimed but never exercised.

---

## P-2 — Extract pricing into a domain policy; kill the three copies

**Where:** `HiringQuoteCalculator.cs:36-50`, `workforceApi.ts:50-61`, `HiringQuoteCalculatorTests.cs:22`

Employer-cost rates (0.21 / 0.06 / 0.09) and platform fees (699 / 149 / 299) live in the backend switch, again in the frontend fallback, and implicitly a third time in the test's expected total. Move them into a named domain type — e.g. `PricingPolicy.For(EmploymentType)` returning `(decimal Rate, Money Fee)` — so there is one authoritative table. The frontend copy disappears entirely once the fallback stops recomputing quotes (issue I-2). Also worth noting: both `switch` expressions carry an unreachable `_` arm for a three-value enum; with a policy table, that discard-with-a-made-up-default (`0.12m`? `299m`? chosen arbitrarily) goes away.

---

## P-3 — Parallelize the dashboard reads

**Where:** `WorkforceDashboardService.cs:9-11`

Three independent awaits run sequentially. Against the in-memory model it's free, but the whole point of the port is that this will someday be SQL/Cosmos/vendor calls — at which point this is three round-trips in series for no reason.

```csharp
var countriesTask = readModel.GetCountriesAsync(ct);
var workersTask = readModel.GetWorkersAsync(ct);
var payrollTask = readModel.GetPayrollRunsAsync(ct);
await Task.WhenAll(countriesTask, workersTask, payrollTask);
```

For a teaching repo, this is a high-signal lesson in exactly the layer where it matters.

---

## P-4 — Tighten `Result<T>` so success can't carry null

**Where:** `src/PeblClone.Application/Common/Result.cs`, `WorkforceController.cs:29`

`Result<T>` exposes a public constructor via the record's primary constructor, so `new Result<HiringQuoteDto>(null, null)` is a "success" with no value. The controller compensates with `!result.IsSuccess || result.Value is null` — a double-check that betrays the type isn't trustworthy. Make the constructor private, mark it `Result<T>.Success(T value)` with a null guard, and give it a `Match`/deconstruct helper:

```csharp
public sealed class Result<T>
{
    private readonly T? _value;
    public string? Error { get; }
    public bool IsSuccess { get; }
    public T Value => IsSuccess ? _value! : throw new InvalidOperationException("No value on failure.");
    ...
}
```

Also consider carrying an error *code* alongside the message so the API can distinguish "unsupported country" (404-ish) from "invalid compensation" (400) instead of flattening everything to 400 with prose.

---

## P-5 — Make the frontend fallback honest and observable

**Where:** `workforceApi.ts`, `App.tsx`

Beyond the correctness fix in I-2, the fallback design deserves a pass:

- Distinguish *network unreachable* (fallback justified) from *server error* (show an error banner; mock data masks real incidents) from *client error* (surface the ProblemDetails message).
- Add an `AbortController` timeout (~5s) so a hung API doesn't leave the dashboard on the loading screen indefinitely.
- The `source` badge is a nice touch — extend it with a retry affordance ("Demo fallback active — Retry") instead of requiring a full reload to reconnect.

---

## P-6 — Filter or disable unavailable engagement types in `QuoteBuilder`

**Where:** `QuoteBuilder.tsx:44-62`

The country and engagement selects are independent, but the domain says they're not (Japan has no contractor management). The UI lets users compose a request the backend is guaranteed to reject. The data needed to prevent this is already in props (`country.employerOfRecordAvailable`, `country.contractorManagementAvailable`) — disable invalid `<option>`s and reset the engagement type when the selected country doesn't support it. Cheap change, removes a whole class of avoidable 400s.

---

## P-7 — Deduplicate `humanize` and harden `StatusBadge` class names

**Where:** `features/formatters.ts:16-18`, `components/StatusBadge.tsx:13-15`

`humanize` is implemented twice, identically. Import it from `formatters`. Separately, `StatusBadge` builds a CSS class from the raw label (`status-${label.toLowerCase()}`); today's labels are single words, but the prop type admits any string — a label with a space would produce two classes. Map known variants to class names explicitly, or slugify.

---

## P-8 — Expand test coverage where behavior is interesting

Current coverage is two happy-ish path tests. The logic most worth pinning down:

- `Worker.OnboardingProgress` — rounding, and the debatable "no milestones ⇒ 100%" rule (`Worker.cs:15-18`). If 100% is intentional (payroll-only workers with nothing to onboard), a test documents it; if not, this is a latent bug for a `Draft` worker with no milestones yet showing a full progress bar.
- `Worker.RequiresAttention` — the High-unresolved-check trigger.
- `HiringQuoteCalculator` — invalid compensation, unsupported country, EOR unavailable (only the contractor branch is tested), and rounding of employer costs.
- Dashboard ordering contracts (attention-first workers, risk-then-due-date alerts) — these are product behavior the UI relies on, and nothing tests them.
- Frontend: zero tests exist. Vitest + Testing Library on `QuoteBuilder` (validation, error state) and `formatters` (the I-3 date bug would have been caught by a single test) would be proportionate.

Also: CI should run `dotnet test` *and* the frontend lint/tests once they exist; today the frontend job only proves the build compiles.

---

## P-9 — Validate the request at the edge

**Where:** `HiringQuoteDtos.cs`, `WorkforceController.cs`

`HiringQuoteRequest` accepts any string for `CountryIsoCode` and `Currency` and any decimal for compensation, deferring everything to hand-rolled checks inside the calculator. Data-annotation attributes (`[Range]`, `[Length(2,2)]`, `[RegularExpression]`) or a small validator would reject malformed input with automatic 400s before the use case runs, and keep the calculator focused on business rules (availability) rather than shape checks. `SingleOrDefault` on ISO lookup (`HiringQuoteCalculator.cs:18`) should also become a dictionary/`FirstOrDefault` — a duplicated ISO code in seed data currently produces a 500, not a 400.

---

## P-10 — Small odds and ends

- **`docs2/` naming** — interview Q&A files sitting next to `docs/`; fold into `docs/` with a subfolder or move out of the product repo.
- **README accuracy** — "This machine does not currently have the .NET SDK installed" is a statement about the author's machine at creation time, not about the project; it will age badly. Replace with plain prerequisites (SDK 10, Node 22).
- **`Directory.Packages.props`** — `xunit.runner.visualstudio` 2.8.2 lags xunit 2.9.3; align them. Consider `xunit.v3` if the repo is meant to model current practice.
- **`.editorconfig`** exists but is minimal — for a teaching repo, shipping the dotnet analyzers severity conventions (`dotnet_diagnostic.*`) would reinforce the `AnalysisLevel=latest` + warnings-as-errors stance.
- **Accessibility** — the onboarding progress bar (`App.tsx:146-148`) uses a styled `span` with `aria-label` on the wrapper; a `role="progressbar"` with `aria-valuenow` would make it real to screen readers. The sidebar `Api`/`Mock` badge conveys state by color+word only — fine, just keep it that way.
