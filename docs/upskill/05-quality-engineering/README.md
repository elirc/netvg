# Quality Engineering

Quality is not “more tests.” It is knowing which failure would hurt, where to catch it, and how to keep the signal useful.

Current coverage is application-layer only:

- Quote tests: `tests/PeblClone.Application.Tests/HiringQuoteCalculatorTests.cs:11-37`.
- Dashboard summary test: `tests/PeblClone.Application.Tests/WorkforceDashboardServiceTests.cs:10-22`.
- CI runs backend tests and frontend build at `.github/workflows/ci.yml:16-33`.

No API integration tests, frontend tests, E2E tests, EF migration tests, auth tests, or observability tests were found.

