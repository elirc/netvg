# 03 Maintainer Communication

## Ask Without Outsourcing Thinking

Poor:

> How do I add auth?

Better:

> I see the API registers controllers but no auth services at `src/PeblClone.Api/Program.cs:7-27`, and the controller has no `[Authorize]` at `src/PeblClone.Api/Controllers/WorkforceController.cs:6-40`. I propose adding JWT bearer auth plus a `CanViewWorkforce` policy. Is that direction compatible with this repo’s goals?

## Bug Report Template

```md
## Repro
1. Run API on `http://localhost:5088`.
2. Run Vite on `5173`.
3. Select Japan + Contractor in the quote form.

## Expected
Backend validation from `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:31-34` should be visible.

## Actual
Client fallback may produce a quote from `src/PeblClone.Web/src/services/workforceApi.ts:43-77`.
```

## Responding To Review

Good:

> Agreed. I moved the rule back into `HiringQuoteCalculator`, added a test beside `tests/PeblClone.Application.Tests/HiringQuoteCalculatorTests.cs:11-37`, and left the UI as display-only.

