using PeblClone.Domain.Workforce;

namespace PeblClone.Application.Workforce;

public sealed record HiringQuoteRequest(
    string CountryIsoCode,
    EmploymentType EmploymentType,
    decimal MonthlyCompensation,
    string Currency);

public sealed record HiringQuoteDto(
    string CountryIsoCode,
    string CountryName,
    EmploymentType EmploymentType,
    decimal BaseCompensation,
    decimal EstimatedEmployerCosts,
    decimal PlatformFee,
    decimal TotalMonthlyCost,
    string Currency,
    int EstimatedOnboardingDays,
    IReadOnlyList<string> ComplianceRequirements);
