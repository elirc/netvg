# Architecture And Patterns

Architecture is the set of boundaries that make change cheaper or more expensive. In this repo, the strongest boundary is API -> Application -> Domain with Infrastructure as an adapter; see `src/PeblClone.Api/Program.cs:20-22`, `src/PeblClone.Application/Workforce/IWorkforceReadModel.cs:5-12`, and `src/PeblClone.Infrastructure/DependencyInjection.cs:9-12`.

Read:

1. `01-boundaries-and-layers.md`
2. `02-data-model-and-persistence.md`
3. `03-validation-auth-and-permissions.md`
4. `04-side-effects-async-and-reliability.md`
5. `05-pattern-catalog.md`
6. `06-architecture-critique.md`

