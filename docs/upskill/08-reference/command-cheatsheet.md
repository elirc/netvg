# Command Cheatsheet

| Command | Status | Evidence |
| --- | --- | --- |
| `rg --files` | Verified | Used for inventory. |
| `git status --short -- .` | Verified | Returned `?? ./`, repo untracked in parent Git root. |
| `dotnet --info` | Failed | `dotnet` command not recognized locally. |
| `dotnet restore PeblClone.sln` | Inferred | Solution at `PeblClone.sln:1-58`; CI restore at `.github/workflows/ci.yml:16`. |
| `dotnet build PeblClone.sln --configuration Release --no-restore` | Inferred | CI build at `.github/workflows/ci.yml:17`. |
| `dotnet test PeblClone.sln --configuration Release --no-build` | Inferred | CI test at `.github/workflows/ci.yml:18`. |
| `dotnet run --project src/PeblClone.Api` | Inferred | Launch URL at `src/PeblClone.Api/Properties/launchSettings.json:4-11`. |
| `cd src/PeblClone.Web; npm install` | Inferred | `package.json` exists at `src/PeblClone.Web/package.json:1-25`. |
| `npm run dev` | Inferred | Script at `src/PeblClone.Web/package.json:7`; Vite port at `src/PeblClone.Web/vite.config.ts:6-9`. |
| `npm run build` | Inferred | Script at `src/PeblClone.Web/package.json:8`; CI at `.github/workflows/ci.yml:32-33`. |
| `npm run lint` | Inferred | Script at `src/PeblClone.Web/package.json:10`; not run in CI. |
| `npm install --package-lock-only --ignore-scripts` | Verified pass | Generated lockfile without `node_modules`. |

No migrations, Docker, codegen, or formatting commands were found.

