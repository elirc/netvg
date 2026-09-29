# 01 Good First Tickets

## Ticket 1: Add Negative Compensation Test
**Difficulty:** Easy **Time:** 30m **Skills:** xUnit, validation  
**Story:** As a maintainer, I want negative quote inputs tested so pricing stays safe.  
**Read first:** `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:12-15`; `tests/PeblClone.Application.Tests/HiringQuoteCalculatorTests.cs:11-37`  
**Acceptance criteria:** - [ ] Test fails if failure branch removed. - [ ] No product code refactor.

## Ticket 2: Add Unknown Country Test
**Difficulty:** Easy **Time:** 30m **Skills:** tests  
**Read first:** `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:17-24`  
**Acceptance criteria:** - [ ] Asserts `IsSuccess` false. - [ ] Error mentions country code.

## Ticket 3: Test Money Currency Guard
**Difficulty:** Easy **Time:** 45m **Skills:** domain tests  
**Read first:** `src/PeblClone.Domain/Workforce/Money.cs:7-14`  
**Acceptance criteria:** - [ ] Adding USD and EUR throws. - [ ] Adding same currency succeeds.

## Ticket 4: Test Worker Attention By High Risk
**Difficulty:** Easy **Time:** 45m **Skills:** domain invariants  
**Read first:** `src/PeblClone.Domain/Workforce/Worker.cs:20-22`  
**Acceptance criteria:** - [ ] Active worker with unresolved high check requires attention. - [ ] Low risk does not.

## Ticket 5: Add Payroll Ordering Test
**Difficulty:** Easy **Time:** 45m **Skills:** application tests  
**Read first:** `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:47-50`  
**Acceptance criteria:** - [ ] Dashboard payroll runs ordered by deadline.

## Ticket 6: Add Country Sorting Test
**Difficulty:** Easy **Time:** 45m **Skills:** application tests  
**Read first:** `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:37-41`  
**Acceptance criteria:** - [ ] Active countries appear before inactive. - [ ] Scores sort descending within group.

## Ticket 7: Add README Link To Upskill Lab
**Difficulty:** Easy **Time:** 15m **Skills:** docs  
**Read first:** `README.md:1-46`; `docs/upskill/README.md:1-33`  
**Acceptance criteria:** - [ ] Link docs without changing product behavior.

## Ticket 8: Add Lint To CI
**Difficulty:** Easy **Time:** 30m **Skills:** CI  
**Read first:** `src/PeblClone.Web/package.json:6-10`; `.github/workflows/ci.yml:20-33`  
**Acceptance criteria:** - [ ] CI runs `npm run lint` or explains why config must come first.

## Ticket 9: Add API Error Message To Quote UI
**Difficulty:** Medium **Time:** 1h **Skills:** React state  
**Read first:** `src/PeblClone.Web/src/features/QuoteBuilder.tsx:17-31`; `src/PeblClone.Web/src/services/workforceApi.ts:37-77`  
**Acceptance criteria:** - [ ] User sees quote failure when fallback disabled. - [ ] Loading state still clears.

## Ticket 10: Add AbortController To Dashboard Fetch
**Difficulty:** Medium **Time:** 1h **Skills:** React effects  
**Read first:** `src/PeblClone.Web/src/App.tsx:14-35`; `src/PeblClone.Web/src/services/workforceApi.ts:6-21`  
**Acceptance criteria:** - [ ] Request cancels on unmount. - [ ] State updates remain guarded.

## Ticket 11: Add OpenAPI Contract Check
**Difficulty:** Medium **Time:** 1h **Skills:** API contracts  
**Read first:** `src/PeblClone.Api/Program.cs:24-36`; `src/PeblClone.Application/Workforce/DashboardDtos.cs:5-68`  
**Acceptance criteria:** - [ ] Document command to fetch OpenAPI. - [ ] No generated files committed unless intentional.

## Ticket 12: Add Health Check Documentation
**Difficulty:** Easy **Time:** 20m **Skills:** ops docs  
**Read first:** `src/PeblClone.Api/Program.cs:27,40`  
**Acceptance criteria:** - [ ] Explain `/health` and local expected response.

## Ticket 13: Add Security Header Test Plan
**Difficulty:** Easy **Time:** 30m **Skills:** security thinking  
**Read first:** `src/PeblClone.Api/Extensions/SecurityHeadersExtensions.cs:7-15`  
**Acceptance criteria:** - [ ] Docs list expected headers and one future integration test.

## Ticket 14: Extract Quote Fee Policy Constants
**Difficulty:** Medium **Time:** 1h **Skills:** refactoring  
**Read first:** `src/PeblClone.Application/Workforce/HiringQuoteCalculator.cs:36-50`  
**Acceptance criteria:** - [ ] Tests still pass. - [ ] No behavior change.

## Ticket 15: Add No-Workers Dashboard Test
**Difficulty:** Medium **Time:** 1h **Skills:** test doubles  
**Read first:** `src/PeblClone.Application/Workforce/WorkforceDashboardService.cs:20-28`  
**Acceptance criteria:** - [ ] Average score is 100 when no active countries. - [ ] Uses controlled fake read model.

## Ticket 16: Add Frontend Empty State Story
**Difficulty:** Medium **Time:** 1h **Skills:** UI states  
**Read first:** `src/PeblClone.Web/src/App.tsx:42-47`; `src/PeblClone.Web/src/App.tsx:165-175`  
**Acceptance criteria:** - [ ] Empty alerts do not render a broken panel. - [ ] No API changes.

