# 04 Runtime And Tooling Map

## Backend Runtime

| Concern | Evidence | What it means |
| --- | --- | --- |
| SDK pin | `global.json:1-6` | Local builds expect .NET SDK `10.0.100` or a compatible feature roll-forward. |
| Target framework | `Directory.Build.props:1-10` | All projects inherit `net10.0`, nullable refs, implicit usings, warnings as errors, latest analysis. |
| Central packages | `Directory.Packages.props:1-10` | Package versions are centralized; project files should not add ad hoc versions. |
| API URL | `src/PeblClone.Api/Properties/launchSettings.json:4-11` | Development API runs on `http://localhost:5088`. |
| CORS | `src/PeblClone.Api/Program.cs:7-18` | Browser requests allowed from local Vite origins only. |
| Health/OpenAPI | `src/PeblClone.Api/Program.cs:24-40` | Controllers, OpenAPI, ProblemDetails, health checks are registered. |

## Frontend Runtime

| Concern | Evidence | What it means |
| --- | --- | --- |
| Bundler | `src/PeblClone.Web/vite.config.ts:1-10` | Vite + React plugin, dev server on port `5173`. |
| Scripts | `src/PeblClone.Web/package.json:6-10` | `dev`, `build`, `preview`, and `lint` are declared. |
| Env | `src/PeblClone.Web/.env.example:1` | `VITE_API_BASE_URL` points the browser to the API. |
| Contracts | `src/PeblClone.Web/src/services/workforceTypes.ts:7-97` | TS interfaces mirror C# DTOs manually. |

## Runtime Boundaries

- Browser boundary: `src/PeblClone.Web/src/services/workforceApi.ts:6-21` wraps `fetch`.
- HTTP boundary: `src/PeblClone.Api/Controllers/WorkforceController.cs:12-40` maps requests to application services.
- Application boundary: `src/PeblClone.Application/Workforce/IWorkforceReadModel.cs:5-12` hides storage details.
- Adapter boundary: `src/PeblClone.Infrastructure/DependencyInjection.cs:9-12` binds the port to in-memory data.

## Verification Notes

No Dockerfile, compose file, `nuget.config`, `Makefile`, `Taskfile`, `CONTRIBUTING`, `SECURITY`, or `LICENSE` was found in `rg --files` output.

