# Fable — Code Review of AtlasWork (PeblClone)

Review date: 2026-07-01. Scope: full repo — .NET backend (Domain / Application / Infrastructure / Api), React frontend (PeblClone.Web), tests, solution/build files, CI.

## Contents

| File | What's in it |
|---|---|
| [issues.md](issues.md) | Concrete bugs and defects, ordered by severity, with file/line references |
| [improvements.md](improvements.md) | Design and quality improvements — not bugs, but things worth changing |

## Overall verdict

The codebase is well-shaped for its stated teaching purpose. The layering is honest (Domain has no dependencies, Application owns use cases and ports, Infrastructure adapts, controllers stay thin), records are immutable, central package management and `TreatWarningsAsErrors` are on, and the frontend keeps API typing and fallback behavior in one place as the README promises.

That said, the review found **one build-blocking defect** (a malformed project-type GUID in `PeblClone.sln` that prevents the solution from loading), **a cluster of correctness bugs around the frontend mock fallback** (it silently bypasses backend validation and business rules), and **a systemic looseness around money** (currencies are echoed, summed, and compared inconsistently — the `Money` type exists but its one safety guarantee is never used where it matters).

## Findings at a glance

| # | Severity | Finding |
|---|---|---|
| I-1 | Blocker | Invalid project-type GUID in `PeblClone.sln` — solution fails to parse |
| I-2 | High | `createQuote` mock fallback bypasses backend validation and availability rules |
| I-3 | High | `formatDate` has a UTC/local off-by-one-day bug for all date-only strings |
| I-4 | High | Dashboard payroll total sums `Money.Amount` across unchecked currencies |
| I-5 | Medium | Quote calculator echoes request currency without conversion; flat USD fees mislabeled |
| I-6 | Medium | `QuoteBuilder` submit has no error handling — unhandled rejection path |
| I-7 | Medium | `npm run lint` is broken — no ESLint 9 flat config exists |
| I-8 | Medium | Unhandled exceptions return bare 500s — `UseExceptionHandler` never wired |
| I-9 | Low | Seed "Today" frozen at process start in a singleton |
| I-10 | Low | `Money` equality vs `Add` treat currency casing inconsistently |
| I-11 | Low | `UseHttpsRedirection` with no HTTPS endpoint configured |
| P-1..P-10 | — | See improvements.md (test coupling, duplicated pricing logic, sequential awaits, `Result<T>` shape, and more) |
