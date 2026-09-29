# Issues

Concrete defects, ordered by severity. Line references are to the current state of `main`.

---

## I-1 (Blocker) — Invalid project-type GUID breaks the solution file

**Where:** `PeblClone.sln:5,7,9,11,13`

Every project entry uses the project-type GUID `{9A19103F-16F7-4668-BE54-9A1E7A4BA76D1}`. The well-known GUID for SDK-style C# projects is `{9A19103F-16F7-4668-BE54-9A1E7A4BA76D}` — the version in the file has an extra `1` appended, making the final segment 13 hex digits. That is not a valid GUID, so Visual Studio and MSBuild's solution parser will fail to load `PeblClone.sln`, which in turn breaks `dotnet restore PeblClone.sln` / `dotnet build PeblClone.sln` — the exact commands the CI workflow runs (`.github/workflows/ci.yml`). CI has presumably never gone green on the backend job.

**Fix:** Replace all five occurrences with `{9A19103F-16F7-4668-BE54-9A1E7A4BA76D}` (or regenerate the solution with `dotnet new sln` + `dotnet sln add`).

---

## I-2 (High) — `createQuote` mock fallback bypasses backend validation and business rules

**Where:** `src/PeblClone.Web/src/services/workforceApi.ts:37-77`

The catch block in `createQuote` triggers on **any** error — including an HTTP 400 the backend deliberately returned. The fallback then recomputes a quote locally from `mockDashboard`, which means:

1. **Validation bypass.** The backend rejects `MonthlyCompensation <= 0` (`HiringQuoteCalculator.cs:12-15`). The fallback happily produces a quote for 0 (or a negative number). Clearing the compensation input yields `Number('') === 0` (`QuoteBuilder.tsx:71`), the API says 400, and the user is shown a confident quote of `$699` for a $0 hire.
2. **Business-rule bypass.** The backend rejects Contractor hires in Japan (`ContractorManagementAvailable: false`). The fallback doesn't check availability at all (`workforceApi.ts:44`), so selecting Japan + Contractor while the API is up shows the user a quote the platform would never honor.
3. **Logic duplication.** The employer-cost rates and platform fees are hardcoded a second time in the fallback (`workforceApi.ts:50-61`), guaranteeing drift the first time backend pricing changes.

**Fix:** Only fall back on *network* failure (e.g., `TypeError` from `fetch`), never on a 4xx/5xx response — parse the ProblemDetails body and surface `detail` to the user instead. If a demo fallback for quotes must exist, mirror the availability checks or (better) delete the local pricing math and show a "demo mode — quotes unavailable" state.

---

## I-3 (High) — `formatDate` shifts date-only values by one day for users west of UTC

**Where:** `src/PeblClone.Web/src/features/formatters.ts:9-14`

The backend serializes `DateOnly` as `"2026-06-12"`. `new Date('2026-06-12')` parses that as **UTC midnight**, and `Intl.DateTimeFormat` then renders it in the **local** timezone. For any user in the Americas, every due date, deadline, and activity date on the dashboard displays one day early (`Jun 11` instead of `Jun 12`). Payroll approval deadlines are exactly the kind of data where an off-by-one day matters.

**Fix:** Parse date-only strings as local dates:

```ts
export function formatDate(value: string): string {
  const [year, month, day] = value.split('-').map(Number);
  return new Intl.DateTimeFormat('en-US', { month: 'short', day: 'numeric' })
    .format(new Date(year, month - 1, day));
}
```

(or keep `new Date(value)` and pass `timeZone: 'UTC'` to `Intl.DateTimeFormat`).

---

## I-4 (High) — Dashboard payroll total sums amounts across unchecked currencies

**Where:** `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:25`

```csharp
MonthlyPayrollUsd: workers.Sum(worker => worker.MonthlyCost.Amount)
```

This sums raw `decimal` amounts and labels the result USD. It works today only because every seed worker happens to be USD. The moment a worker is stored in EUR or JPY (the domain explicitly supports it — `Money` carries a currency, and `CountryProfile.PrimaryCurrency` exists), the headline dashboard metric silently becomes garbage. The irony: `Money.Add` (`Money.cs:7-15`) exists precisely to prevent cross-currency addition, and this — the one place in the codebase that adds money — doesn't use it.

**Fix:** Fold with `Money.Add` so a currency mismatch throws loudly, or filter/convert explicitly and document the assumption. At minimum:

```csharp
MonthlyPayrollUsd: workers
    .Select(w => w.MonthlyCost)
    .Aggregate(Money.Zero("USD"), (acc, m) => acc.Add(m)).Amount
```

---

## I-5 (Medium) — Quote calculator echoes the request currency without conversion

**Where:** `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:44-63`

`platformFee` is a flat constant (699 / 149 / 299 — implicitly USD), yet the response stamps whatever currency the caller sent (`request.Currency`, line 63) on the whole quote. `HiringQuoteCalculatorTests.cs:17` even passes `"EUR"` and asserts a total of `12_799m` — so the test enshrines a quote of "€12,799" that mixes a EUR salary with a $699 fee. The currency is also never validated (`"FOO"` is accepted) and never compared against `country.PrimaryCurrency`.

**Fix:** Either constrain quotes to USD (validate and reject anything else — honest for a demo), or model fees as `Money` and require an FX port to convert. Validate the currency code against a known set either way.

---

## I-6 (Medium) — `QuoteBuilder` submit has no error handling

**Where:** `src/PeblClone.Web/src/features/QuoteBuilder.tsx:17-32`

`handleSubmit` is `try/finally` with no `catch`. `createQuote` rethrows when the API fails *and* the country is missing from mock data (`workforceApi.ts:46-48`). In that path the rejection is unhandled: the console gets an error, the user gets nothing — the button just returns to "Calculate" with no explanation. There is no error state in the component at all.

**Fix:** Add a `catch`, store an `error` string in state, and render it near the form. This becomes essential once I-2 is fixed and 400s properly reject instead of silently falling back.

---

## I-7 (Medium) — `npm run lint` is broken: no ESLint config exists

**Where:** `src/PeblClone.Web/package.json:10,20-22`

The project depends on `eslint@^9` and `typescript-eslint@^8` and defines `"lint": "eslint ."`, but there is no `eslint.config.js` (or any ESLint config) anywhere in `PeblClone.Web`. ESLint 9 requires flat config and exits with an error when none is found. Dead script, dead devDependencies — and CI never runs lint, so nothing catches it (`.github/workflows/ci.yml` frontend job runs only `npm ci` + `npm run build`).

**Fix:** Add a flat `eslint.config.js` using `typescript-eslint`'s recommended preset, then add `npm run lint` to the CI frontend job.

---

## I-8 (Medium) — Unhandled exceptions return bare 500s despite `AddProblemDetails`

**Where:** `src/PeblClone.Api/Program.cs:26,29-43`

`builder.Services.AddProblemDetails()` is registered but `app.UseExceptionHandler()` is never added to the pipeline. In production, an unhandled exception (e.g., the `InvalidOperationException` from `Money.Add`, or `SingleOrDefault` throwing on a duplicate ISO code) produces an empty 500 with no ProblemDetails body — inconsistent with the 400s the API carefully shapes in `WorkforceController.cs:31-36`.

**Fix:** Add `app.UseExceptionHandler();` (parameterless overload works with `AddProblemDetails`) before the other middleware.

---

## I-9 (Low) — Seed clock frozen at process start in a singleton

**Where:** `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:8`

```csharp
private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);
```

`Today` is captured once when the type initializes, and the read model is registered as a singleton (`DependencyInjection.cs:11`). On a long-running process the "due in 3 days" alerts silently become overdue-but-still-labeled-fresh. It also makes anything derived from the seed non-deterministic across days, which matters because the application tests assert against this exact data (see P-1).

**Fix:** Inject `TimeProvider` (built into .NET 8+) and build the seed per resolution, or accept the freeze but document it as intentional demo behavior.

---

## I-10 (Low) — `Money` equality and `Add` disagree about currency casing

**Where:** `src/PeblClone.Domain/Workforce/Money.cs`

`Add` compares currencies with `OrdinalIgnoreCase` (line 9), but record equality uses default ordinal string comparison — so `new Money(5, "usd") != new Money(5, "USD")` while `Add` happily combines them. A value object should have one notion of identity.

**Fix:** Normalize in the constructor (`Currency = Currency.ToUpperInvariant()`) and use plain ordinal comparison everywhere; optionally validate it's a 3-letter code.

---

## I-11 (Low) — `UseHttpsRedirection` with no HTTPS endpoint configured

**Where:** `src/PeblClone.Api/Program.cs:38`, `src/PeblClone.Api/Properties/launchSettings.json`

The launch profile exposes only `http://localhost:5088`. With no HTTPS port to redirect to, the middleware logs "Failed to determine the https port for redirect" on every request and no-ops. Additionally, the CORS policy allows only `http://localhost:5173`; if HTTPS redirection ever *did* engage, the redirected origin story would break the dev flow.

**Fix:** Either add an `https` applicationUrl to the launch profile, or remove `UseHttpsRedirection()` for this local-demo API and note why.

---

## Data-consistency nits (no code change strictly required)

- **Mock vs. seed drift:** `mockDashboard.ts` claims `openComplianceItems: 5` but lists only 3 alerts; the backend seed produces 5 alerts. Mock dates are fixed June 2026 strings that are already in the past as of July 2026, while backend seed dates are relative to "today." The mock also omits Germany's payroll run present in the seed. Harmless individually, but the demo's "mock" and "api" modes visibly disagree.
- **Future-dated activity:** `BuildActivity` (`WorkforceDashboardService.cs:109-119`) uses `StartDate` as the event date; Aiko Tanaka's start date is 18 days in the future, so the feed reports a "moved to Onboarding" event that hasn't happened yet.
