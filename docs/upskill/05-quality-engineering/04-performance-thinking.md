# 04 Performance Thinking

Measure first. This repo is small, so the point is learning where performance pressure would appear.

| Domain | Likely hotspot | Evidence | How to measure |
| --- | --- | --- | --- |
| Server CPU/allocation | Dashboard materialization | `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:12-50` | Benchmark or profile with realistic worker counts. |
| Server latency | Serial reads | `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:9-11` | Add timers/logs per read when backing store is real. |
| DB | Future N+1 | `src/PeblClone.Domain/Workforce/Worker.cs:20-22` over compliance checks | EF query logging after EF exists. |
| Network | Dashboard payload size | DTO shape at `src/PeblClone.Application/Workforce/DashboardDtos.cs:5-68` | Browser network panel. |
| React render | Direct table render | `src/PeblClone.Web/src/App.tsx:132-152` | React Profiler with hundreds of workers. |
| Bundle | Vite build | `src/PeblClone.Web/package.json:8`, `.github/workflows/ci.yml:32-33` | Vite bundle analyzer if added. |

Failure modes:

- Optimizing seed data paths before measuring real data.
- Adding caching before defining invalidation and tenant isolation.
- Parallelizing `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:9-11` before knowing if downstream can handle concurrency.

Drill: Design a benchmark input with 10,000 workers and 5 compliance checks each. Which line allocates most?

