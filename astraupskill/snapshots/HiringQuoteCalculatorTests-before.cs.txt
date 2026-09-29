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
}
