# 9. Verification

Run these commands from the project root [`../`](../). They are the direct project-relative checks for this course. The environment used for the recorded evidence was Node 22.16 and .NET 10.

```powershell
dotnet restore tests/PeblClone.Application.Tests/PeblClone.Application.Tests.csproj
dotnet test tests/PeblClone.Application.Tests/PeblClone.Application.Tests.csproj -c Release --no-restore
dotnet build src/PeblClone.Api/PeblClone.Api.csproj -c Release
cd src/PeblClone.Web
npm ci
node --experimental-strip-types --test tests/request-gate.test.mjs
npm run build
```

The first .NET command restores the exact application test project. The second runs the real [`HiringQuoteCalculatorTests.cs`](../tests/PeblClone.Application.Tests/HiringQuoteCalculatorTests.cs) suite in Release mode without restoring again. The expected recorded root check is `root-dotnet-test-02`: 14 xUnit tests pass with TRX evidence. That is the delivered count; a later fix pass added `CalculateAsync_RoundsEmployerCostsAwayFromZeroAtAnExactMidpoint` (a three-row `[Theory]`) and `CalculateAsync_RejectsCompensationAboveTheDemoCap`, so re-running today reports 18/18 (observed 2026-09-17: `Passed!  - Failed:     0, Passed:    18, Skipped:     0, Total:    18, Duration: 1 s`). The same pass removed the meaningless `checked` keyword from the `decimal` total and corrected the claim about it in chapter 03. It covers a successful Germany EOR quote, Japan contractor rejection, undefined enum, nonpositive compensation, whitespace normalization, deterministic rounding, independent requirements, invalid currency, and invalid country syntax.

From `src/PeblClone.Web`, `npm ci` installs the lockfile dependencies. The Node command runs [`tests/request-gate.test.mjs`](../src/PeblClone.Web/tests/request-gate.test.mjs) against the real [`requestGate.ts`](../src/PeblClone.Web/src/services/requestGate.ts). The expected root check is `root-request-tests-01`: 3 tests pass, covering delayed overlap, edit invalidation plus fresh retry, and unmount cleanup isolation. This is a finite gate harness with deferred promises; it does not mount React and therefore does not claim to prove DOM rendering or screen-reader behavior. `npm run build` is `root-web-build-03` and proves the web project compiles and bundles. Do not invent an `npm run typecheck` check or orchestration flags when those scripts are absent.

If the native runtime is unavailable, use the project's supported .NET 10 installation or set `DOTNET_ROLL_FORWARD=Major` only when the machine's runtime policy requires it; record the environment fact. A missing dependency is a limitation, never a pass. Keep the browser's hardcoded USD separate from EUR direct API fixtures. Neither value proves conversion, payroll, tax, legal compliance, provider confirmation, persistence, authentication, deployment, or audit history.

Read the [portable results](evidence/results.json), [application test log](evidence/root-dotnet-test-02.log), [Node test log](evidence/root-request-tests-01.log), [web build](evidence/root-web-build-03.log), and [API build](evidence/root-api-build-02.log). These files preserve the recorded run; their absolute working directories describe the review environment, while the commands above work from this project's root. Inspect application-tests.trx alongside the log when checking discovery counts. The application suite includes thirteen calculator cases and one existing dashboard case.

The API initially failed restore because Microsoft.OpenApi 2.0.0 triggered warning-as-error NU1903. The final project explicitly selects 2.7.5 in Directory.Packages.props and references it in the API project. This is the patched 2.x release identified in [Microsoft's advisory](https://github.com/microsoft/OpenAPI.NET/security/advisories/GHSA-v5pm-xwqc-g5wc); warning enforcement stays enabled. The final API build restored dependencies and passed. This targeted repair is not a claim that every dependency has been independently security-audited.

**Exercise 8.** Write a verification note for a run where the .NET test and Node gate tests pass, the web build passes, and no React component is mounted. Include the three check labels, the 14/3 counts, the build limitation, and the no-conversion currency statement. Explain why this is strong evidence for the request gate but not payroll truth.
