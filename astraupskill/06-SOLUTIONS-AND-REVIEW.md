# 6. Solutions and review

Use this key after attempting Exercises 0 through 8. Exercise 0’s first rejecting owner is the C# calculator: it validates availability after the controller has bound the request. The final renderer is `QuoteBuilder`, where the parsed ProblemDetails `detail` becomes a `role="alert"`. No total exists on that path.

Exercise 1 maps ownership as follows: country and currency normalization belong to the calculator; country availability belongs to the calculator’s profile rule; HTTP status belongs to the controller; stale-response suppression belongs to the component’s `RequestGate`; and the mock-dashboard fallback belongs to `getDashboard`. The server must still enforce engagement availability because a direct caller can bypass a select option and a stale browser can submit an old value.

Exercise 2 classifies blank country and `EU!` currency as shape failures, both returned as failed results and then HTTP 400. `JP` Contractor is a business rejection with a meaningful availability detail and HTTP 400. A refused fetch is infrastructure failure: the promise rejects and the quote form shows retry. `DE` EOR is success. Only dashboard loading may return mock data because that product contract labels it; quote creation must represent a server calculation.

Exercise 3’s EOR calculation is `100.005 * 0.21 = 21.00105`, rounded to `21.00` with `AwayFromZero`; the fee is `699`; the unrounded compensation remains `100.005`; total is `820.005`. Output is country `DE` and currency `EUR`. The UI’s USD default is irrelevant because this is a direct API fixture. There is no conversion between EUR and USD.

Exercise 4’s useful assertions are `result.Value!.Currency.Should().Be("EUR")` and, after the Node test, an explanation that the harness invokes `RequestGate` with promises but never renders React. `npm run build` from `src/PeblClone.Web` supplies the TypeScript and JSX compilation check. A root check recorded 3 Node tests passing and the web build passing; it did not claim a mounted React test.

Exercise 5 has this timeline. Submit DE: token 1, loading true, quote and error cleared. Edit to JP: `invalidate` moves the gate beyond token 1, loading false, and quote/error remain empty. Retry JP Contractor: token 3, loading true, empty result. The first response or rejection sees `isCurrent(1) == false` and cannot commit. The retry’s 400 body becomes an `Error`; token 3 is current, so the alert shows the ProblemDetails detail and `finally` clears loading. The retry shares the form handler.

Exercise 6: if a duplicate ISO code appears in a fake read model, `SingleOrDefault` throws instead of returning a controlled business failure. The `InvalidOperationException` escapes the controller. `Program.cs` registers `AddProblemDetails()` but never calls `UseExceptionHandler()`, so the caller gets an unhandled 500 rather than a 400 (see `fable/issues.md` I-8). That is a data invariant failure, so a production design should detect duplicates at startup or use a dictionary construction that reports the seed problem; it should not silently pick a country. A test should pin the chosen invariant before changing lookup behavior. On cancellation: `CalculateAsync` passes its token only to `readModel.GetCountriesAsync`, and the in-memory adapter ignores it (`InMemoryWorkforceReadModel.cs:164-165` returns `Task.FromResult(Countries)`). Nothing in the calculator calls `ThrowIfCancellationRequested`. Cancellation is plumbing kept ready for a future I/O-backed adapter; it is not delivered behavior, and no test covers it.

Exercise 7's overlapping-request trace permits token 1's initial loading reset and token 2's newer loading reset. Token 2 may commit success and clear loading. Token 1's later success and finally block are both suppressed. In the edit trace, invalidation immediately clears the preview and loading state; the old rejection must not add an error. Gating only try/catch leaves a bug: token 1's finally could stop the spinner while token 2 is still pending.

Exercise 8's verification note should name root-dotnet-test-02 (14 cases), root-request-tests-01 (three cases), and root-web-build-03 (compilation and bundle). No component was mounted, so alert rendering and browser navigation remain untested. EUR and USD are labels on synthetic values with no conversion. The later root-api-build-02 also passed after an OpenAPI dependency patch; a build is not an additional unit test.

For review, inspect four questions in order. Does every externally supplied value have a defined application contract? Does every rejected server response stay rejected in the client? Can a user distinguish loading, stale, success, and failure? Does evidence test the real boundary? Look for exact error fragments, exact decimal assertions, a `NotBeNull()` before dereferencing `Value`, and reference inequality for requirements. Also check that loading is gated in `finally`, edits invalidate, unmount cleanup invalidates, and retry uses the same form.

## Exercise 5 walkthrough (moved from chapter 05)

This is the implementation narration that used to sit inside the practice chapter. Read it after attempting the task list there.

First, make the generic `request<T>` helper read the unsuccessful response body as text. Attempt JSON parsing and prefer `detail`, then `title`, then the raw body. If the body is empty, use `Request failed with status ${response.status}`. Throw an `Error` with that message. This preserves a server explanation such as “Contractor management is not available in Japan.” while still handling a proxy that returns plain text. Do not catch this in `createQuote`; the function should be one thin call:

```ts
export async function createQuote(requestBody: HiringQuoteRequest): Promise<HiringQuote> {
  return request<HiringQuote>('/api/workforce/quotes', {
    method: 'POST',
    body: JSON.stringify(requestBody)
  });
}
```

Next, repair component state. At submit start, call `setIsLoading(true)`, clear `error`, and clear `quote`. Obtain a token from `requestNumber.current.next()`. On success, set the quote only when `isCurrent(token)` is true. In `catch`, use `submitError instanceof Error ? submitError.message : 'The quote could not be calculated.'`; set the alert only for the current token. In `finally`, clear loading only for the current token. A retry button should be `<button type="submit" form="quote-form" disabled={isLoading}>Retry</button>` so it uses the same handler and cannot drift.

Now handle edits. Each country, engagement, and compensation `onChange` calls `invalidate`, updates its value, clears quote and error, and sets loading false. `useRef(new RequestGate())` keeps the token outside render. The cleanup effect calls `invalidate` on unmount. This matters in two races: request one for Germany can finish after request two for Japan, and a response can finish after the user changes compensation. In both cases the old promise may resolve, but it cannot commit state.

Use the browser form’s `USD` value when describing this screen. For a direct API regression test, use `EUR` to check trimming and uppercasing. Both are labels on synthetic numbers. Keep the dashboard’s labeled mock fallback in `getDashboard`; quote creation must not manufacture a success during an outage or business rejection.

### Exercises 5a and 5b

5a, deletion 1 (corrected 2026-10-06; an earlier version of this key gave the wrong answer). With `.ToUpperInvariant()` removed from the `currency` line, `" eur "` becomes `"eur"`. The guard on line 25 rejects any character outside `'A'..'Z'`, and `'e'` is outside it. The calculator therefore returns `Failure("Currency must be a three-letter ISO code.")`, and the **first** assertion fails: `result.IsSuccess.Should().BeTrue()`. The `Currency.Should().Be("EUR")` line is never reached. The earlier key said `IsSuccess` stays true and only the currency assertion fails. That is wrong, because the guard checks the *normalized* variable, so its correctness depends on the upper-casing that comes before it. Lowercase input must therefore stay a valid way to call the API, and only the normalization makes it valid.

5a, deletion 2. With `.ToUpperInvariant()` removed from the `countryCode` line, all 18 tests stay green. The length check accepts `"de"`. The lookup on lines 41-42 compares with `StringComparison.OrdinalIgnoreCase`, and the DTO returns `country.IsoCode` from the profile (line 101), not `countryCode`. Country upper-casing is redundant today, and no test would notice if it were removed. If you want it to be load-bearing, write a test that would catch its removal, or delete it deliberately and say so in review. The same reading shows that country has a length check but no character check: `"D!"` in `CalculateAsync_RejectsInvalidCountrySyntax` is rejected by the **lookup** ("not currently supported"), not by a syntax guard. The test passes only because both messages contain "Country".

5b: `Math.Round(21.105m, 2)` with the default `MidpointRounding.ToEven` returns `21.10`; with `AwayFromZero` it returns `21.11`. The midpoint theory asserts both the expected value and `NotBe(bankersRoundingWouldGive)`, so removing the argument fails on the first assertion with `Expected ... to be 21.11m, but found 21.10m`. A cent of difference between the quote screen and the invoice is a support ticket, not a rounding detail, because the customer compares two numbers that were supposed to be the same number.

### Red-first lab

Corrected 2026-10-06 by reading the snapshot against today's 18-case test file. The earlier key named only three red tests and said the original code threw. **10 of 18 cases go red** against `snapshots/HiringQuoteCalculator-before.cs.txt`:

| Red case(s) | Why the original code fails it |
|---|---|
| `RejectsInvalidEmploymentType` (1) | The original did **not** throw. Its switches had `_ => 0.12m` and `_ => 299m` defaults, so `(EmploymentType)99` produced a *successful* quote, and `IsSuccess.Should().BeFalse()` fails. |
| `NormalizesCodesAndRoundsAwayFromZero` (1) | The lookup compared the raw `" de "`, found no country, and returned a failure, so `IsSuccess.Should().BeTrue()` fails. |
| `RoundsEmployerCostsAwayFromZeroAtAnExactMidpoint` (3) | `Math.Round(x, 2)` used the default `ToEven`, giving 21.10, 7.54 and 4.54. |
| `RejectsCompensationAboveTheDemoCap` (1) | There was no upper bound, so `1_000_000_000_001m` was quoted successfully. |
| `ReturnsIndependentRequirementsCopy` (1) | The DTO received `country.RequiredDocuments` itself, the same static list, so `NotBeSameAs` fails. |
| `RejectsInvalidCurrency` (3) | There was no currency validation, so `"123"`, `"EU!"` and `""` were all echoed back in successful quotes. |

The other 8 stay green: the `DE`/EOR happy path, the `JP` contractor rejection, both non-positive compensation rows, the three country-syntax rows (the raw codes simply fail the lookup, and that message also contains "Country"), and the dashboard test. The original bug class showed only on rejected, untidy or extreme input. The lab cannot be run from a clean clone without copying the snapshot over the source, so treat this table as the prediction to check, not as a recorded run. The line to find is in `snapshots/workforceApi-before.ts.txt`, not the QuoteBuilder snapshot (corrected 2026-10-06). The `catch (error)` in `createQuote` (snapshot lines 43-76) looks the country up in `mockDashboard` and recomputes the rates and fees locally. It rethrows only when the country is missing from the mock. So a Japanese contractor request was rejected by the API with a 400, and the user still saw a plausible monthly total: the mock path never checks contractor availability. `QuoteBuilder-before.tsx.txt` adds the second half of the bug. Its `handleSubmit` is `try`/`finally` with no `catch` and no request gate, so a stale response could still overwrite newer state.
