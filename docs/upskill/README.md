# AtlasWork Upskill Lab

This curriculum turns this repository into a training lab for a junior engineer growing toward mid-level and senior judgment. The product is a small global workforce platform: a React dashboard calls an ASP.NET Core API, which composes workforce, country, payroll, compliance, and hiring quote data through Application and Infrastructure layers. The backend entry point is `src/PeblClone.Api/Program.cs:5-43`; the API routes are in `src/PeblClone.Api/Controllers/WorkforceController.cs:6-40`; the main use cases live in `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:5-120` and `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:6-67`; the React shell is `src/PeblClone.Web/src/App.tsx:9-233`.

Use this lab in four stages:

| Track | Use it this way |
| --- | --- |
| Weekend | Read `00-fast-track.md`, trace dashboard and quote flows, run available checks, make one tiny docs or test change. |
| Two weeks | Work through cartography, stack mental models, annotation drills, and five good-first tickets. |
| Eight weeks | Add cross-layer tests, propose one persistence/auth design, complete review katas, write a small RFC. |
| Ongoing | Revisit the risk register, senior build projects, interview prep, and architecture critique after each change. |

Recommended paths:

| Learner | Path |
| --- | --- |
| Brand-new junior | `00-fast-track.md` -> `01-codebase-cartography/02-file-reading-order.md` -> `04-code-reading-gym/01-annotation-drills.md`. |
| Junior with C# or React familiarity | `01-codebase-cartography/05-key-flows.md` -> `02-stack-and-language-mastery/02-framework-mental-models.md` -> `06-contribution-practice/01-good-first-tickets.md`. |
| Mid-level new to repo | `03-architecture-and-patterns/01-boundaries-and-layers.md` -> `05-quality-engineering/01-testing-strategy.md` -> `06-contribution-practice/02-mid-level-feature-tickets.md`. |
| Senior doing architecture review | `03-architecture-and-patterns/06-architecture-critique.md` -> `08-reference/risk-register.md` -> `06-contribution-practice/03-senior-build-projects.md`. |

Conventions:

- Codebase claims include exact `path:line-line` anchors.
- Illustrative examples are labeled `Illustrative fake code: not from this repo`.
- “Investigate” means the evidence was not present in this repository.
- Drills include self-grading so a learner can continue without asking an AI.

Senior mindset: a junior asks “how do I make it work?”, a mid-level engineer asks “is this the right pattern?”, and a senior asks “what does this commit us to, who pays for the tradeoff, and how do we reduce risk?”

## Verification Notes

- Inventory used `rg --files`.
- JSON and XML metadata parsed successfully.
- `dotnet --info` failed because the SDK is not installed on this machine; see `08-reference/verification-log.md`.

