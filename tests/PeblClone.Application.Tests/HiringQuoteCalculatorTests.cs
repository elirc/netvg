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

    // A genuine midpoint: the product ends in exactly .005, so the third decimal is the tie.
    // MidpointRounding.AwayFromZero gives the first number; .NET's default ToEven would give the
    // second. Removing AwayFromZero from HiringQuoteCalculator turns every row of this theory red.
    [Theory]
    [InlineData(EmploymentType.EmployerOfRecord, 100.5, 21.11, 21.10)]   // 100.5 * 0.21 = 21.105
    [InlineData(EmploymentType.Contractor, 125.75, 7.55, 7.54)]          // 125.75 * 0.06 = 7.545
    [InlineData(EmploymentType.PayrollOnly, 50.5, 4.55, 4.54)]           // 50.5 * 0.09 = 4.545
    public async Task CalculateAsync_RoundsEmployerCostsAwayFromZeroAtAnExactMidpoint(
        EmploymentType employmentType,
        decimal compensation,
        decimal expectedAwayFromZero,
        decimal bankersRoundingWouldGive)
    {
        var result = await new HiringQuoteCalculator(new InMemoryWorkforceReadModel())
            .CalculateAsync(new HiringQuoteRequest("DE", employmentType, compensation, "EUR"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.EstimatedEmployerCosts.Should().Be(expectedAwayFromZero);
        result.Value.EstimatedEmployerCosts.Should().NotBe(bankersRoundingWouldGive);
    }

    // decimal overflow throws OverflowException with or without a `checked` context, but the
    // compensation cap means CalculateAsync rejects the value long before the arithmetic runs.
    [Fact]
    public async Task CalculateAsync_RejectsCompensationAboveTheDemoCap()
    {
        var result = await new HiringQuoteCalculator(new InMemoryWorkforceReadModel())
            .CalculateAsync(
                new HiringQuoteRequest("DE", EmploymentType.EmployerOfRecord, 1_000_000_000_001m, "EUR"),
                CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("within the demo limit");
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
