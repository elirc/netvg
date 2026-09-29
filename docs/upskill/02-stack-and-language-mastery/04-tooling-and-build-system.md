# 04 Tooling And Build System

## .NET Tooling

| Tooling concept | Repo evidence | Transferable lesson |
| --- | --- | --- |
| SDK pinning | `global.json:1-6` | Teams pin SDKs so local, CI, and release builds agree. |
| Target framework | `Directory.Build.props:1-10` | Shared props remove project-by-project drift. |
| Central packages | `Directory.Packages.props:1-10` | Version ownership is centralized; avoid adding versions in `.csproj`. |
| Solution graph | `PeblClone.sln:5-14` | IDE/CI load the same project set. |
| CI backend | `.github/workflows/ci.yml:9-18` | Restore, build, test are the minimum backend gate. |

Failure mode: local `dotnet` is unavailable, so commands are inferred from CI and project files, not verified locally.

## npm And Vite

| Tooling concept | Repo evidence | Transferable lesson |
| --- | --- | --- |
| Scripts | `src/PeblClone.Web/package.json:6-10` | Scripts are the public interface for frontend contributors. |
| Dependencies | `src/PeblClone.Web/package.json:12-24` | React runtime deps are separated from dev tooling. |
| Vite config | `src/PeblClone.Web/vite.config.ts:4-10` | Dev server config is explicit and small. |
| Env config | `src/PeblClone.Web/.env.example:1` | Frontend env variables must be prefixed with `VITE_`. |
| CI frontend | `.github/workflows/ci.yml:20-33` | `npm ci` uses the lockfile for reproducible installs. |

Drill: Why should CI use `npm ci` instead of `npm install`? Use `.github/workflows/ci.yml:32-33` and `src/PeblClone.Web/package-lock.json` as evidence.

## Build Cache And Warnings

`TreatWarningsAsErrors` is enabled at `Directory.Build.props:6`. This raises quality but can make upgrades harder. A senior asks: which warnings should block all builds, and which analyzers need rollout windows?

## Verification Notes

JSON and XML metadata parsed successfully. `npm install --package-lock-only --ignore-scripts` completed with zero reported vulnerabilities. `dotnet --info` failed locally.

