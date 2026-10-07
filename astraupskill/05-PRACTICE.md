# 5. Practice: repair a quote flow

This chapter is a task list, not a walkthrough. The implementation narration that used to live here is now the answer key in [`06-SOLUTIONS-AND-REVIEW.md`](06-SOLUTIONS-AND-REVIEW.md) under "Exercise 5 walkthrough (moved from chapter 05)" — do not read it until you have tried the tasks.

## The scenario

Imagine a branch where the quote form catches every failed `fetch`, calculates a number locally from mock data, and leaves the previous successful quote visible while a new request is pending. A user in Japan asks for contractor management — which the demo profile does not allow — and the screen shows them a real-looking monthly total. Your job is to make a rejected request stay rejected, and to make a slow response unable to overwrite newer UI state.

The files you may change: [`workforceApi.ts`](../src/PeblClone.Web/src/services/workforceApi.ts), [`QuoteBuilder.tsx`](../src/PeblClone.Web/src/features/QuoteBuilder.tsx), [`requestGate.ts`](../src/PeblClone.Web/src/services/requestGate.ts).

## Acceptance criteria

1. A non-OK response produces a thrown `Error` whose message is the server's ProblemDetails `detail`, or `title` if there is no `detail`, or the raw body text if it parses as neither, or `Request failed with status <status>` when the body is empty.
2. `createQuote` contains no `try`/`catch` and no fallback value — it is one call into the shared helper.
3. During a pending request the previous quote and the previous error are both cleared, and a loading state is visible.
4. A response that belongs to a superseded request cannot set the quote, cannot set the error, and cannot clear the loading flag.
5. Editing any field (country, engagement, compensation) supersedes the in-flight request immediately and clears quote, error and loading.
6. Unmounting supersedes the in-flight request.
7. Retry re-uses the same submit handler and the same validation (`type="submit" form="quote-form"`), not a second code path.
8. `getDashboard`'s labelled mock fallback stays exactly as it is. A read-only view may degrade to labelled sample data; a write must never synthesize success.

## Required checks

Write or extend these before you call the task done:

- Node gate harness (`src/PeblClone.Web`): a deferred-promise test where request 1 resolves *after* request 2 and must not commit.
- Node gate harness: a test where `invalidate()` runs between submit and resolution.
- `dotnet test` still green (`tests/PeblClone.Application.Tests`) — nothing in this task touches C#, and a red suite means you edited the wrong thing.

## Exercise 5a (code): make a normalizer test fail first

`CalculateAsync_NormalizesCodesAndRoundsAwayFromZero` passes `" de "` and `" eur "`. Make two deletions on a scratch branch, one at a time. Restore the code after each.

1. Delete `.ToUpperInvariant()` from the **`currency`** line (`HiringQuoteCalculator.cs:18`). Before running anything, write down which assertion in the normalization test fails first. Look at the guard on line 25 before you decide.
2. Delete `.ToUpperInvariant()` from the **`countryCode`** line (line 17) instead. Predict how many of the 18 tests go red.

**Goal:** discover which normalization step is load-bearing and which one is redundant today.

**Check:**

```powershell
dotnet test tests/PeblClone.Application.Tests/PeblClone.Application.Tests.csproj -c Release --filter FullyQualifiedName~Normalizes
dotnet test tests/PeblClone.Application.Tests/PeblClone.Application.Tests.csproj -c Release
```

Compare the failing assertion name and the red/green count with your two predictions. Then write one sentence explaining any prediction that was wrong.

## Exercise 5b (code): prove the rounding mode matters

Open `CalculateAsync_RoundsEmployerCostsAwayFromZeroAtAnExactMidpoint`. Predict, for `100.5` at the EOR rate `0.21`, what `Math.Round(x, 2)` returns *without* `MidpointRounding.AwayFromZero`. Then remove the argument from `HiringQuoteCalculator.cs`, run

```powershell
dotnet test tests/PeblClone.Application.Tests/PeblClone.Application.Tests.csproj -c Release --filter FullyQualifiedName~Midpoint
```

and compare the reported actual value with your prediction. Restore the argument. Write one sentence on why a total that is one cent low is a support ticket rather than a rounding detail.

## Lab: red-first from the snapshots

`snapshots/` holds the exact pre-change files. Run the delivered checks against the original code and record the failures before you look at any answer:

1. Copy `snapshots/HiringQuoteCalculator-before.cs.txt` over `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs` and `snapshots/QuoteBuilder-before.tsx.txt` over `src/PeblClone.Web/src/features/QuoteBuilder.tsx`.
2. Predict, in writing, which of the C# tests go red and why.
3. Run `dotnet test tests/PeblClone.Application.Tests/PeblClone.Application.Tests.csproj -c Release` and record every failing test name and message.
4. Read `snapshots/workforceApi-before.ts.txt` and find the `catch` in `createQuote` that manufactures a quote from mock data after a failed request. The quote is manufactured in the API helper, not in `QuoteBuilder`. Then read `snapshots/QuoteBuilder-before.tsx.txt`: its `try`/`finally` has no `catch` and no request gate. Write down what a user in Japan saw.
5. Restore both delivered files, re-run to green, and write one sentence per failure naming the line of the repair that fixed it.

**Exercise 5.** Write pseudocode for the state timeline when a user submits `DE`, immediately edits the country to `JP`, then retries as `JP` Contractor. Include every gate that prevents the first response from changing the second attempt. State what the user sees during the edit and after the rejected retry.

**Exercise 6.** (This prompt previously appeared only at the bottom of the answer key; it moved here on 2026-10-06.) Review two paths in the current source and say, for each one, what is delivered behavior and what is a future extension. (a) Suppose the read model held two profiles with `IsoCode` `"DE"`. What does the lookup at `HiringQuoteCalculator.cs:41-42` do, and what HTTP response would a caller get? (b) Follow the `CancellationToken` passed to `CalculateAsync`. Which line observes it? Name the test or startup invariant you would add before changing either behavior.
