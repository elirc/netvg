# 04 Refactor And Design Katas

| Kata | Anchor | Task | Self-grade |
| --- | --- | --- | --- |
| Boundary leak | `src/PeblClone.Web/src/services/workforceApi.ts:50-75` | Remove duplicated quote math design. | Strong keeps demo behavior without policy drift. |
| Outbox/eventing | `src/PeblClone.Domain/Workforce/ComplianceCheck.cs:1-10` | Design compliance event publishing. | Strong includes idempotency and rollback. |
| Split module | `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:7-120` | Decide when to split mappers. | Strong avoids premature abstraction. |
| Improve type safety | `src/PeblClone.Domain/Workforce/Money.cs:3-15` | Add currency/amount invariants. | Strong handles migration and tests. |
| Design migration | `src/PeblClone.Infrastructure/Workforce/InMemoryWorkforceReadModel.cs:6-172` | Move to EF without changing controllers. | Strong keeps port stable. |
| Reduce N+1 | `src/PeblClone.Domain/Workforce/Worker.cs:20-22` | Design EF query loading compliance checks. | Strong measures and tests query count. |
| Write RFC | `src/PeblClone.Api/Controllers/WorkforceController.cs:20-40` | RFC for quote audit logging. | Strong covers privacy and retention. |
| Review flawed PR | `.github/workflows/ci.yml:16-33` | PR removes tests/build step. | Strong suggests faster safer alternative. |

