# 6. Solutions and review

Use this key after attempting Exercises 0 through 8. Exercise 0’s first rejecting owner is the C# calculator: it validates availability after the controller has bound the request. The final renderer is `QuoteBuilder`, where the parsed ProblemDetails `detail` becomes a `role="alert"`. No total exists on that path.

Exercise 1 maps ownership as follows: country and currency normalization belong to the calculator; country availability belongs to the calculator’s profile rule; HTTP status belongs to the controller; stale-response suppression belongs to the component’s `RequestGate`; and the mock-dashboard fallback belongs to `getDashboard`. The server must still enforce engagement availability because a direct caller can bypass a select option and a stale browser can submit an old value.

Exercise 2 classifies blank country and `EU!` currency as shape failures, both returned as failed results and then HTTP 400. `JP` Contractor is a business rejection with a meaningful availability detail and HTTP 400. A refused fetch is infrastructure failure: the promise rejects and the quote form shows retry. `DE` EOR is success. Only dashboard loading may return mock data because that product contract labels it; quote creation must represent a server calculation.

Exercise 3’s EOR calculation is `100.005 * 0.21 = 21.00105`, rounded to `21.00` with `AwayFromZero`; the fee is `699`; the unrounded compensation remains `100.005`; total is `820.005`. Output is country `DE` and currency `EUR`. The UI’s USD default is irrelevant because this is a direct API fixture. There is no conversion between EUR and USD.

Exercise 4’s useful assertions are `result.Value!.Currency.Should().Be("EUR")` and, after the Node test, an explanation that the harness invokes `RequestGate` with promises but never renders React. `npm run build` from `src/PeblClone.Web` supplies the TypeScript and JSX compilation check. A root check recorded 3 Node tests passing and the web build passing; it did not claim a mounted React test.

Exercise 5 has this timeline. Submit DE: token 1, loading true, quote and error cleared. Edit to JP: `invalidate` moves the gate beyond token 1, loading false, and quote/error remain empty. Retry JP Contractor: token 3, loading true, empty result. The first response or rejection sees `isCurrent(1) == false` and cannot commit. The retry’s 400 body becomes an `Error`; token 3 is current, so the alert shows the ProblemDetails detail and `finally` clears loading. The retry shares the form handler.

Exercise 6: if a duplicate ISO code appears in a fake read model, `SingleOrDefault` throws instead of returning a controlled business failure. That is a data invariant failure, so a production design should detect duplicates at startup or use a dictionary construction that reports the seed problem; it should not silently pick a country. A test should pin the chosen invariant before changing lookup behavior.

Exercise 7's overlapping-request trace permits token 1's initial loading reset and token 2's newer loading reset. Token 2 may commit success and clear loading. Token 1's later success and finally block are both suppressed. In the edit trace, invalidation immediately clears the preview and loading state; the old rejection must not add an error. Gating only try/catch leaves a bug: token 1's finally could stop the spinner while token 2 is still pending.

Exercise 8's verification note should name root-dotnet-test-02 (14 cases), root-request-tests-01 (three cases), and root-web-build-03 (compilation and bundle). No component was mounted, so alert rendering and browser navigation remain untested. EUR and USD are labels on synthetic values with no conversion. The later root-api-build-02 also passed after an OpenAPI dependency patch; a build is not an additional unit test.

For review, inspect four questions in order. Does every externally supplied value have a defined application contract? Does every rejected server response stay rejected in the client? Can a user distinguish loading, stale, success, and failure? Does evidence test the real boundary? Look for exact error fragments, exact decimal assertions, a `NotBeNull()` before dereferencing `Value`, and reference inequality for requirements. Also check that loading is gated in `finally`, edits invalidate, unmount cleanup invalidates, and retry uses the same form.

**Exercise 6.** Perform the duplicate-profile and cancellation review described above against the current source. Write down which part is delivered behavior and which part is a future extension, then name the test or invariant you would add before implementation.

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

5a: with `.ToUpperInvariant()` removed from the `currency` line, the failing assertion is `result.Value.Currency.Should().Be("EUR")`, reporting `Expected result.Value.Currency to be "EUR", but "eur" differs near "eur"`. `IsSuccess` stays true, because the three-letter length check passes and the character-range check runs against the *normalized* variable — which is exactly why asserting only `IsSuccess` would miss it.

5b: `Math.Round(21.105m, 2)` with the default `MidpointRounding.ToEven` returns `21.10`; with `AwayFromZero` it returns `21.11`. The midpoint theory asserts both the expected value and `NotBe(bankersRoundingWouldGive)`, so removing the argument fails on the first assertion with `Expected ... to be 21.11m, but found 21.10m`. A cent of difference between the quote screen and the invoice is a support ticket, not a rounding detail, because the customer compares two numbers that were supposed to be the same number.

### Red-first lab

Against `snapshots/HiringQuoteCalculator-before.cs.txt` the added boundary tests are the ones that go red: the undefined-`EmploymentType` theory (the original reached the `switch` and threw instead of returning a failure), the normalization test (the original did not trim or upper-case), and the midpoint theory. The happy-path `DE`/EOR test stays green — the original bug class only showed on rejected or untidy input. In `snapshots/QuoteBuilder-before.tsx.txt` the line to find is the `catch` that fell back to a locally computed quote: a Japanese contractor request was rejected by the API with a 400 and the user still saw a plausible monthly total.
