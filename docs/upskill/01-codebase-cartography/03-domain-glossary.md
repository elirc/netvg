# 03 Domain Glossary

| Term | Meaning in this repo | Anchors |
| --- | --- | --- |
| Worker | A person being managed across country, role, employment type, cost, onboarding, and compliance. | `src/PeblClone.Domain/Workforce/Worker.cs:3-13` |
| Employment type | How the worker is engaged: EOR, contractor, or payroll-only. | `src/PeblClone.Domain/Workforce/EmploymentType.cs:1-8`, `src/PeblClone.Web/src/services/workforceTypes.ts:1` |
| Worker status | Lifecycle state such as onboarding, active, or needs attention. | `src/PeblClone.Domain/Workforce/WorkerStatus.cs:1-9`, `src/PeblClone.Web/src/services/workforceTypes.ts:3` |
| Country profile | Country readiness data: availability, payroll lead time, score, documents, benefits. | `src/PeblClone.Domain/Workforce/CountryProfile.cs:3-13` |
| Compliance check | A risk item with country, owner, due date, resolution state. | `src/PeblClone.Domain/Workforce/ComplianceCheck.cs:1-10` |
| Risk level | Low, Medium, or High. High unresolved checks make a worker require attention. | `src/PeblClone.Domain/Workforce/ComplianceRiskLevel.cs:1-8`, `src/PeblClone.Domain/Workforce/Worker.cs:20-22` |
| Payroll run | Country-level payroll period, deadline, amount, worker count, approval state. | `src/PeblClone.Domain/Workforce/PayrollRun.cs:1-10` |
| Hiring quote | Estimate of monthly cost and onboarding requirements for a country/engagement. | `src/PeblClone.Application/Workforce/HiringQuoteDtos.cs:5-21` |
| Read model | Application port that supplies countries, workers, payroll runs. | `src/PeblClone.Application/Workforce/IWorkforceReadModel.cs:5-12` |
| Mock fallback | Frontend behavior that returns local data if API fetch fails. | `src/PeblClone.Web/src/services/workforceApi.ts:23-35,43-77` |

## Drill

Pick “RequiresAttention.” Explain the invariant from `src/PeblClone.Domain/Workforce/Worker.cs:20-22`, then find where the UI displays the consequence at `src/PeblClone.Web/src/App.tsx:100-103` and `src/PeblClone.Web/src/App.tsx:142-144`.

