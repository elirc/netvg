# 4. Test the real boundaries

The calculator tests use the real `HiringQuoteCalculator` and `InMemoryWorkforceReadModel`. Their placement is [`tests/PeblClone.Application.Tests/HiringQuoteCalculatorTests.cs`](../tests/PeblClone.Application.Tests/HiringQuoteCalculatorTests.cs), in the `PeblClone.Application.Tests` namespace. The listing below is the fixture **as delivered** (13 cases). Since then, two methods have been added to the real file between `CalculateAsync_NormalizesCodesAndRoundsAwayFromZero` and `CalculateAsync_ReturnsIndependentRequirementsCopy`: `CalculateAsync_RoundsEmployerCostsAwayFromZeroAtAnExactMidpoint`, a three-row `[Theory]`, and `CalculateAsync_RejectsCompensationAboveTheDemoCap`. Open the file for those two (note added 2026-10-06). Keep new tests in the same file and class, so that names, packages and placement match the project:

```csharp
using FluentAssertions;
using PeblClone.Application.Workforce;
using PeblClone.Domain.Workforce;
using PeblClone.Infrastructure.Workforce;
using Xunit;

namespace PeblClone.Application.Tests;

public sealed class HiringQuoteCalculatorTests
{
    [Fact]
    public async Task CalculateAsync_ReturnsQuote_WhenCountrySupportsEmploymentType()
    {
        var calculator = new HiringQuoteCalculator(new InMemoryWorkforceReadModel());
        var result = await calculator.CalculateAsync(
            new HiringQuoteRequest("DE", EmploymentType.EmployerOfRecord, 10_000, "EUR"),
            CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.TotalMonthlyCost.Should().Be(12_799m);
        result.Value.EstimatedOnboardingDays.Should().Be(19);
    }

    [Fact]
    public async Task CalculateAsync_ReturnsFailure_WhenCountryDoesNotSupportContractors()
    {
        var calculator = new HiringQuoteCalculator(new InMemoryWorkforceReadModel());
        var result = await calculator.CalculateAsync(
            new HiringQuoteRequest("JP", EmploymentType.Contractor, 10_000, "JPY"),
            CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Contractor management is not available");
    }

    [Theory]
    [InlineData((EmploymentType)99)]
    public async Task CalculateAsync_RejectsInvalidEmploymentType(EmploymentType employmentType)
    {
        var result = await new HiringQuoteCalculator(new InMemoryWorkforceReadModel())
            .CalculateAsync(new HiringQuoteRequest("DE", employmentType, 10_000, "EUR"), CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        if ((int)employmentType == 99) result.Error.Should().Contain("Employment type");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CalculateAsync_RejectsNonPositiveCompensation(decimal compensation)
    {
        var result = await new HiringQuoteCalculator(new InMemoryWorkforceReadModel())
            .CalculateAsync(new HiringQuoteRequest("DE", EmploymentType.EmployerOfRecord, compensation, "EUR"), CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task CalculateAsync_NormalizesCodesAndRoundsAwayFromZero()
    {
        var result = await new HiringQuoteCalculator(new InMemoryWorkforceReadModel())
            .CalculateAsync(new HiringQuoteRequest(" de ", EmploymentType.EmployerOfRecord, 100.005m, " eur "), CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        result.Value!.CountryIsoCode.Should().Be("DE");
        result.Value.Currency.Should().Be("EUR");
        result.Value.EstimatedEmployerCosts.Should().Be(21.00m);
        result.Value.TotalMonthlyCost.Should().Be(820.005m);
    }

    [Fact]
    public async Task CalculateAsync_ReturnsIndependentRequirementsCopy()
    {
        var result = await new HiringQuoteCalculator(new InMemoryWorkforceReadModel())
            .CalculateAsync(new HiringQuoteRequest("DE", EmploymentType.EmployerOfRecord, 10_000, "EUR"), CancellationToken.None);
        result.Value!.ComplianceRequirements.Should().NotBeSameAs(
            (await new InMemoryWorkforceReadModel().GetCountriesAsync(CancellationToken.None)).Single(c => c.IsoCode == "DE").RequiredDocuments);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("EU!")]
    [InlineData("")]
    public async Task CalculateAsync_RejectsInvalidCurrency(string currency)
    {
        var result = await new HiringQuoteCalculator(new InMemoryWorkforceReadModel())
            .CalculateAsync(new HiringQuoteRequest("DE", EmploymentType.EmployerOfRecord, 10_000, currency), CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Currency");
    }

    [Theory]
    [InlineData("D")]
    [InlineData("DEU")]
    [InlineData("D!")]
    public async Task CalculateAsync_RejectsInvalidCountrySyntax(string country)
    {
        var result = await new HiringQuoteCalculator(new InMemoryWorkforceReadModel())
            .CalculateAsync(new HiringQuoteRequest(country, EmploymentType.EmployerOfRecord, 10_000, "EUR"), CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Country");
    }
}
```

This fixture checks exact totals, availability text, enum guarding, compensation rejection, normalization, ordinary rounding, copied requirements, and invalid inputs. The original rounding sample (`100.005m`) is not a midpoint, so on its own it does not distinguish AwayFromZero from ToEven; `CalculateAsync_RoundsEmployerCostsAwayFromZeroAtAnExactMidpoint` was added for exactly that, with three products that land on an exact half-cent. The two-character D! sample reaches unsupported-country lookup rather than a letter-only syntax gate. It does not prove tax or legal correctness. Run `dotnet restore tests/PeblClone.Application.Tests/PeblClone.Application.Tests.csproj`, then `dotnet test tests/PeblClone.Application.Tests/PeblClone.Application.Tests.csproj -c Release --no-restore` from [`../`](../). The delivered suite had 14 xUnit cases passing with TRX: 13 calculator cases shown above and one existing dashboard composition case. A later fix pass added the three-row midpoint theory and a compensation-ceiling fact, so the current count is 18 — observed on 2026-09-17 as `Passed!  - Failed:     0, Passed:    18, Skipped:     0, Total:    18`.

The browser ordering check is [`src/PeblClone.Web/tests/request-gate.test.mjs`](../src/PeblClone.Web/tests/request-gate.test.mjs). It imports the same [`requestGate.ts`](../src/PeblClone.Web/src/services/requestGate.ts) class and uses deferred promises. Three Node tests prove an old response cannot replace a newer quote, edit invalidation suppresses an old rejection and permits retry, and cleanup invalidation suppresses an unmounted completion without affecting another gate. It is a finite harness; it does not mount React. Run it with Node 22.16 using `node --experimental-strip-types --test tests/request-gate.test.mjs` after changing directory to `src/PeblClone.Web`. Reset mock implementations in any future component suite, not just call counts.

**Exercise 4.** Add one assertion to the successful C# test for `Currency == "EUR"`, then explain why a passing Node gate test cannot prove that `QuoteBuilder` rendered an alert. Name the additional build check needed for TypeScript and JSX compilation.
