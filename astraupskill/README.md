# Workforce quote course

This course follows one workforce quote from a browser form to a C# calculator and back to visible UI state. It is written against the project at [`../`](../) and names the executable files that a learner should open. Read chapters 1 through 7 in order, then run [`VERIFICATION.md`](VERIFICATION.md) from the project root.

The exercise is deliberately small. The browser form in [`QuoteBuilder.tsx`](../src/PeblClone.Web/src/features/QuoteBuilder.tsx) sends monthly compensation, an engagement type, a country code, and the hardcoded currency label `USD`. [`workforceApi.ts`](../src/PeblClone.Web/src/services/workforceApi.ts) posts that request to `/api/workforce/quotes`. The application calculator in [`HiringQuoteCalculator.cs`](../src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs) validates and normalizes the request, reads country capabilities, computes a synthetic estimate, and returns a DTO. The controller maps expected business failure to HTTP 400 ProblemDetails. A quote shown in the form is therefore an accepted demo calculation, not payroll, tax, employment-law, provider, or currency-conversion truth.

The currency field is a label. The C# code accepts alphabetic three-letter labels such as `EUR`, returns the normalized label, and performs no exchange-rate lookup. The UI currently hardcodes `USD`. Direct API and unit-test examples use `EUR` so normalization is visible. Do not describe either example as a converted amount.

## Prerequisites and glossary

**Prerequisites.** You should be able to read C# methods and `async`/`await`, and read (not necessarily write) React function components with hooks. No EF Core, no SQL and no database knowledge is needed — this project has no persistence at all, which is also its main limitation as a CRUD course: treat it as an elective on input validation, HTTP error contracts and async races.

**What you need installed.**

| Tool | Version used | Unlocks |
|---|---|---|
| .NET SDK 10.x (10.0.302 here) | projects target `net10.0` | `dotnet build`, `dotnet test` (18 xUnit cases) |
| Node 22.x | | `node --test src/PeblClone.Web/tests/request-gate.test.mjs` (3 gate cases) and `npm run build` |
| A browser | any | manually exercising the form; there is no automated browser check |

No database server, no Docker, no Python.

**Time estimate (about 3–5 hours, one or two sittings).**

| Chapter | Estimate |
|---|---|
| 01 Codebase map | 20 min |
| 02 Contracts and business rejection | 45 min |
| 03 Worked change | 45 min |
| 04 Testing and debugging | 40 min |
| 05 Practice (task list, 2 code exercises, red-first lab) | 90 min |
| 06 Solutions and review | 30 min |
| 07 Trace lab | 30 min |
| VERIFICATION | 10 min |

**Glossary.**

| Term | What it means here | Where in this project |
|---|---|---|
| **EOR (employer of record)** | A company that legally employs someone on your behalf in a country where you have no entity. One of the three `EmploymentType` values, and the most expensive rate in the demo table. | `EmploymentType` in `src/PeblClone.Domain/Workforce/EmploymentType.cs` |
| **Contractor management / payroll only** | The other two `EmploymentType` values: paying an independent contractor, and running payroll for someone you already employ. | `Contractor` and `PayrollOnly` in `EmploymentType.cs` |
| **`Result<T>`** | This project's "expected failure" return type: either a value or an error message. Used instead of exceptions for input a user can legitimately get wrong. | `src/PeblClone.Application/Common/Result.cs` |
| **ProblemDetails** | The standard JSON error body ASP.NET Core returns (RFC 9457): `title`, `detail`, `status`. The client prefers `detail` because that is the human-readable sentence. | the `BadRequest(new ProblemDetails { ... })` in `src/PeblClone.Api/Controllers/WorkforceController.cs` |
| **Shape / transport validation** | "Is this request even well-formed?" — two-letter country, three alphabetic letters of currency, a defined enum value, a positive number. Runs before any lookup. | the first guards in `CalculateAsync`, `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs` |
| **Business rejection** | A well-formed request the rules say no to, such as contractor management in Japan. Also a 400, but for a different reason. | the `ContractorManagementAvailable` and EOR checks in `HiringQuoteCalculator.cs` |
| **Normalization** | Turning `" de "` into `"DE"` once, at the boundary, so no later line has to wonder about case or spaces. | `Trim().ToUpperInvariant()` on country and currency in `HiringQuoteCalculator.cs` |
| **`Enum.IsDefined`** | The check that stops JSON like `"employmentType": 99` from becoming a valid-looking enum value. C# enums are just integers underneath and will happily hold 99. | `if (!Enum.IsDefined(request.EmploymentType))` in `HiringQuoteCalculator.cs` |
| **Midpoint rounding / `AwayFromZero`** | What to do with exactly half a cent. .NET's default rounds to the even digit; money usually rounds up. See the sidebar in chapter 02. | `Math.Round(..., 2, MidpointRounding.AwayFromZero)` in `HiringQuoteCalculator.cs` |
| **Request gate (`RequestGate`)** | A counter held in a `useRef` that stamps each submit with a token, so a response that belongs to a superseded submit can be identified and ignored. | `RequestGate` in `src/PeblClone.Web/src/services/requestGate.ts` |
| **Stale response / race** | An older request finishing after a newer one. Without a gate, the older answer overwrites the newer screen. | the `isCurrent(...)` guard in `src/PeblClone.Web/src/features/QuoteBuilder.tsx` |
| **Mock fallback** | Returning invented sample data when a call fails. Allowed for the labelled dashboard read; never for quote creation, because a manufactured success is a lie about money. | `source: 'mock'` in `getDashboard`, `src/PeblClone.Web/src/services/workforceApi.ts` |

The nine-file learning path has eight reading checkpoints plus this README:

1. [`01-CODEBASE-MAP.md`](01-CODEBASE-MAP.md) maps fields and boundaries, then traces a rejected Japan contractor request.
2. [`02-CONCEPTS.md`](02-CONCEPTS.md) separates transport validation, business rejection, infrastructure failure, and the dashboard-only mock fallback.
3. [`03-WORKED-CHANGE.md`](03-WORKED-CHANGE.md) reads the calculator and frontend request gate line by line, including decimal rounding and defensive copying.
4. [`04-TESTING-AND-DEBUGGING.md`](04-TESTING-AND-DEBUGGING.md) gives complete test placement and commands for the real calculator and the finite Node gate harness.
5. [`05-PRACTICE.md`](05-PRACTICE.md) states the repair task, its acceptance criteria and required checks, adds two code exercises and a red-first snapshot lab; the implementation walkthrough now lives in chapter 06.
6. [`06-SOLUTIONS-AND-REVIEW.md`](06-SOLUTIONS-AND-REVIEW.md) provides the answer key for all nine exercises and a review rubric.
7. [`07-TRACE-LAB.md`](07-TRACE-LAB.md) traces successful, rejected, delayed, edited, and unmounted requests with concrete values.
8. [`VERIFICATION.md`](VERIFICATION.md) records reproducible root-relative commands and the evidence limits of each check.

Study by keeping three questions beside the code: which boundary owns this decision, what observable behavior proves it, and what remains outside the demo contract? Those questions prevent a UI symptom from being “fixed” with a second pricing formula and prevent a successful HTTP status from being mistaken for real-world payroll certainty.

**Exercise 0.** Before opening a test, predict the path for `JP`, `Contractor`, `10000`, `EUR`, including the status and visible UI state. Write the first file that can reject the request and the last file that can render the explanation. The answer is developed in chapters 1, 2, and 7.
