# 3. Read the worked change

Open [`HiringQuoteCalculator.cs`](../src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs) and read `CalculateAsync` in gates. The first gate handles a null request. The next two lines normalize the values once:

```csharp
var countryCode = request.CountryIsoCode?.Trim().ToUpperInvariant();
var currency = request.Currency?.Trim().ToUpperInvariant();
```

The following checks reject malformed values before the read model is touched. Country must be exactly two characters. Currency must be exactly three alphabetic characters; `EU!` fails the character predicate even though its length is three. `Enum.IsDefined(request.EmploymentType)` prevents an undefined integer such as 99 from reaching the switch. Compensation must be positive and within `1_000_000_000_000m`, a teaching guard rather than a salary policy.

After `GetCountriesAsync`, lookup compares the stored ISO code case-insensitively. Availability checks use the original enum value, and the switch selects synthetic rates: EOR `0.21m` and fee `699m`, Contractor `0.06m` and `149m`, PayrollOnly `0.09m` and `299m`. Costs use `Math.Round(..., 2, MidpointRounding.AwayFromZero)`. The total is a plain `decimal` addition: **`decimal` arithmetic throws `OverflowException` on overflow regardless of the `checked`/`unchecked` context** — unlike `int`/`long`, where `checked` is required to get an exception instead of silent wraparound. The `try`/`catch` is what turns that throw into a `Result` failure. (An earlier draft of this chapter said `checked` is what detects `decimal` overflow; that was wrong, and the keyword has been removed from the code with a comment in its place.) With the `1_000_000_000_000m` compensation ceiling enforced a few lines above, the `catch (OverflowException)` is unreachable today; it is kept, and commented, as defence in depth for the day the ceiling moves. For `100.005m` EOR, the cost is `21.00105`, displayed and asserted as `21.00`; the total remains `820.005m` because only employer costs are rounded. Requirements use `country.RequiredDocuments.ToArray()`, so the DTO does not expose the shared profile array.

The frontend companion is [`QuoteBuilder.tsx`](../src/PeblClone.Web/src/features/QuoteBuilder.tsx). `const requestNumber = useRef(new RequestGate());` gives the component a stable mutable gate without causing renders. Submit calls `next()`, clears stale quote and error, then updates only when `isCurrent(currentRequest)` is true. `finally` is gated too, so an old completion cannot turn off loading for a newer request. Each input handler calls `invalidate`, clears result state, and sets loading false. The effect cleanup invalidates on unmount. The retry button submits `form="quote-form"`, sharing the same handler and validation.

**Exercise 3.** In a scratch copy, annotate each calculator line as normalization, transport guard, lookup, availability, arithmetic, or projection. Then calculate the EOR result for `DE`, `100.005`, and `EUR`, including employer costs, fee, total, normalized currency, and why the UI’s USD default does not affect this direct API example.
