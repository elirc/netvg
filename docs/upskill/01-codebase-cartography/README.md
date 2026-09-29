# Codebase Cartography

Cartography means making a map before making a change. In this repo, start with the runtime entry points, then trace to use cases, contracts, data adapters, tests, and UI.

| Map | Start |
| --- | --- |
| System map | `01-system-map.md` |
| Reading order | `02-file-reading-order.md` |
| Domain vocabulary | `03-domain-glossary.md` |
| Runtime/tooling | `04-runtime-and-tooling-map.md` |
| End-to-end flows | `05-key-flows.md` |

Verification anchor: the project graph is visible from `PeblClone.sln:5-14`, C# defaults from `Directory.Build.props:1-10`, frontend scripts from `src/PeblClone.Web/package.json:6-10`, and CI from `.github/workflows/ci.yml:8-33`.

