# 7. Trace lab: values and state

Use the following hand trace for a direct request with country `DE`, EmployerOfRecord, compensation `10000`, and currency `EUR`. The browser screen itself sends USD, so this trace represents a calculator or API test. EUR remains a label.

| Boundary | Value or state | Observable decision |
| --- | --- | --- |
| form or caller | `DE`, `EmployerOfRecord`, `10000`, `EUR` | request is submitted |
| JSON body | four named fields | `POST /api/workforce/quotes` |
| controller | `HiringQuoteRequest` | calculator is delegated to |
| calculator | normalized `DE`, `EUR` | shape gates pass |
| read model | Germany profile | EOR is available |
| arithmetic | `10000 + 2100.00 + 699` | total `12799.00` |
| projection | copied requirements, lead time plus 7 | DTO succeeds |
| controller and client | HTTP 200, JSON DTO | quote renders total and details |

The same trace with `JP`, Contractor, `10000`, `JPY` stops at the availability gate. The calculator never selects the contractor rate and fee. The controller maps failure to 400 ProblemDetails. `request` reads the body, extracts `detail`, and throws. `QuoteBuilder` catches that error only for the current request and renders its text in the alert. There is no fallback quote. If the response body is plain text, the helper keeps that text; if it is empty, it uses the status message.

Now trace a race. Submit Germany as request token 1. Before it resolves, submit Japan as token 2. The second request resolves first and commits its result or error; its `finally` clears loading. When token 1 resolves, `RequestGate.isCurrent(1)` is false, so token 1 cannot set quote, error, or loading. This ordering guard is in [`requestGate.ts`](../src/PeblClone.Web/src/services/requestGate.ts), while the delayed-promise proof is in [`request-gate.test.mjs`](../src/PeblClone.Web/tests/request-gate.test.mjs).

For an edit race, submit a quote, then change compensation. The input handler invalidates the gate, clears quote and error, and sets loading false. The old response may still arrive, but its token is stale. The empty state remains honest until the user submits the new value. For unmount, the cleanup effect invalidates the gate. A late completion cannot update an abandoned component. A separate component instance owns a separate `useRef(new RequestGate())`, so unmounting one form does not suppress another form’s current request.

**Exercise 7.** Draw arrows for every state mutation in the overlap trace: submit token 1, submit token 2, token 2 success, token 1 success. Add a second drawing for input edit followed by old rejection. Mark each mutation as allowed or suppressed and explain why `finally` needs the same gate check as `try`.

## The shape gates in the table, in code

The "shape gates pass" row of the trace is this block, from `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:17-38`. Country and currency are normalised first, so `de`/`eur` reach the same gates as `DE`/`EUR`, and every failure is a `Result` value rather than an exception.

```csharp
        var countryCode = request.CountryIsoCode?.Trim().ToUpperInvariant();
        var currency = request.Currency?.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(countryCode) || countryCode.Length != 2)
        {
            return Result<HiringQuoteDto>.Failure("Country must be a two-letter ISO code.");
        }

        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3 || currency.Any(character => character is < 'A' or > 'Z'))
        {
            return Result<HiringQuoteDto>.Failure("Currency must be a three-letter ISO code.");
        }

        if (!Enum.IsDefined(request.EmploymentType))
        {
            return Result<HiringQuoteDto>.Failure("Employment type is not supported.");
        }

        if (request.MonthlyCompensation <= 0 || request.MonthlyCompensation > 1_000_000_000_000m)
        {
            return Result<HiringQuoteDto>.Failure("Monthly compensation must be greater than zero and within the demo limit.");
        }
```

