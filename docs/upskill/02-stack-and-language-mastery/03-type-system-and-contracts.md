# 03 Type System And Contracts

## C# Records And Immutability

Domain and DTO shapes use records:

- `Worker` is a sealed positional record at `src/PeblClone.Domain/Workforce/Worker.cs:3-13`.
- `CountryProfile` is a sealed positional record at `src/PeblClone.Domain/Workforce/CountryProfile.cs:3-13`.
- `WorkforceDashboardDto` and child DTOs are records at `src/PeblClone.Application/Workforce/DashboardDtos.cs:5-68`.
- `HiringQuoteRequest` and `HiringQuoteDto` are records at `src/PeblClone.Application/Workforce/HiringQuoteDtos.cs:5-21`.

Why it matters: records give value-style equality and concise shape declarations. They are good for immutable data transfer. They are not automatic validation; `Money` still needs a currency guard in `src/PeblClone.Domain/Workforce/Money.cs:7-14`.

Failure modes:

- Positional record constructor order becomes a contract. Reordering fields can break callers.
- `IReadOnlyList<T>` prevents mutation through the property but does not guarantee the underlying list is immutable.

## Nullable Reference Types

Nullable reference types are enabled at `Directory.Build.props:3-5`. This means `T?` in `Result<T>` at `src/PeblClone.Application/Common/Result.cs:3` carries meaning: a failed result can have no value.

Why it matters: nullability is a design tool, not just compiler noise. The controller checks both `IsSuccess` and `Value is null` at `src/PeblClone.Api/Controllers/WorkforceController.cs:29-36`.

Drill: Explain why `Result<T>.Failure` returns `default` at `src/PeblClone.Application/Common/Result.cs:9` and how misuse might still happen.

## Enums And Union Types

C# enums:

- `EmploymentType` at `src/PeblClone.Domain/Workforce/EmploymentType.cs:1-8`.
- `WorkerStatus` at `src/PeblClone.Domain/Workforce/WorkerStatus.cs:1-9`.
- `ComplianceRiskLevel` at `src/PeblClone.Domain/Workforce/ComplianceRiskLevel.cs:1-8`.

TypeScript string unions mirror them at `src/PeblClone.Web/src/services/workforceTypes.ts:1-5`.

Why it matters: API serialization and frontend typing must agree. A new C# enum value needs TS updates and UI rendering updates.

Failure modes:

- Contract drift: TS types are manual, not generated from OpenAPI.
- Unknown enum values from the server can break UI assumptions.

## Pattern Matching And Switches

The quote calculator uses switch expressions for rates and platform fees at `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:36-50`.

Why it matters: switches make policy tables visible. But hard-coded policy can become brittle if pricing changes often.

Drill: Add a fake `Intern` employment type mentally. What files must change? Start with `src/PeblClone.Domain/Workforce/EmploymentType.cs:1-8`, `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:36-50`, and `src/PeblClone.Web/src/services/workforceTypes.ts:1`.

## TypeScript Interfaces

The frontend API contract is declared at `src/PeblClone.Web/src/services/workforceTypes.ts:7-97`. These interfaces mirror C# DTOs at `src/PeblClone.Application/Workforce/DashboardDtos.cs:5-68` and `src/PeblClone.Application/Workforce/HiringQuoteDtos.cs:5-21`.

Why it matters: TypeScript protects the UI after data arrives, but it does not validate runtime JSON. For production, consider generated clients or schema validation.

## Verification Notes

No C# generics beyond `Result<T>` at `src/PeblClone.Application/Common/Result.cs:3-10`; no `required`, `init`, structs, spans, or dynamic/unsafe usage found.

