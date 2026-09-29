using PeblClone.Application.Common;
using PeblClone.Domain.Workforce;

namespace PeblClone.Application.Workforce;

public sealed class HiringQuoteCalculator(IWorkforceReadModel readModel) : IHiringQuoteCalculator
{
    public async Task<Result<HiringQuoteDto>> CalculateAsync(
        HiringQuoteRequest request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result<HiringQuoteDto>.Failure("A quote request is required.");
        }

        var countryCode = request.CountryIsoCode?.Trim().ToUpperInvariant();
        var currency = request.Currency?.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(countryCode) || countryCode.Length != 2)
        {
            return Result<HiringQuoteDto>.Failure("Country must be a two-letter ISO code.");
        }

        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3 || currency.Any(character => character is < 'A' or > 'Z'))
        {
            return Result<HiringQuoteDto>.Failure("Currency must be a three-letter ISO code.");
        }

        if (!Enum.IsDefined(request.EmploymentType))
        {
            return Result<HiringQuoteDto>.Failure("Employment type is not supported.");
        }

        if (request.MonthlyCompensation <= 0 || request.MonthlyCompensation > 1_000_000_000_000m)
        {
            return Result<HiringQuoteDto>.Failure("Monthly compensation must be greater than zero and within the demo limit.");
        }

        var countries = await readModel.GetCountriesAsync(cancellationToken);
        var country = countries.SingleOrDefault(item =>
            item.IsoCode.Equals(countryCode, StringComparison.OrdinalIgnoreCase));

        if (country is null)
        {
            return Result<HiringQuoteDto>.Failure($"Country '{request.CountryIsoCode}' is not currently supported.");
        }

        if (request.EmploymentType == EmploymentType.EmployerOfRecord && !country.EmployerOfRecordAvailable)
        {
            return Result<HiringQuoteDto>.Failure($"Employer-of-record hiring is not available in {country.Name}.");
        }

        if (request.EmploymentType == EmploymentType.Contractor && !country.ContractorManagementAvailable)
        {
            return Result<HiringQuoteDto>.Failure($"Contractor management is not available in {country.Name}.");
        }

        var employerCostRate = request.EmploymentType switch
        {
            EmploymentType.EmployerOfRecord => 0.21m,
            EmploymentType.Contractor => 0.06m,
            EmploymentType.PayrollOnly => 0.09m,
            _ => throw new ArgumentOutOfRangeException(
                nameof(request.EmploymentType),
                request.EmploymentType,
                "Unreachable: guarded by Enum.IsDefined above.")
        };

        var platformFee = request.EmploymentType switch
        {
            EmploymentType.EmployerOfRecord => 699m,
            EmploymentType.Contractor => 149m,
            EmploymentType.PayrollOnly => 299m,
            _ => throw new ArgumentOutOfRangeException(
                nameof(request.EmploymentType),
                request.EmploymentType,
                "Unreachable: guarded by Enum.IsDefined above.")
        };

        decimal estimatedEmployerCosts;
        decimal total;
        try
        {
            // Money defaults to MidpointRounding.ToEven ("banker's rounding") in .NET; for a
            // customer-facing total we round half away from zero instead, so 21.105 becomes 21.11.
            estimatedEmployerCosts = Math.Round(request.MonthlyCompensation * employerCostRate, 2, MidpointRounding.AwayFromZero);

            // No `checked` here: decimal arithmetic always throws OverflowException on overflow,
            // whatever the checked/unchecked context (that context only affects int/long/etc.).
            // With the 1e12 compensation cap above this catch is unreachable today; it is kept as
            // defence in depth for the day someone raises or removes that cap.
            total = request.MonthlyCompensation + estimatedEmployerCosts + platformFee;
        }
        catch (OverflowException)
        {
            return Result<HiringQuoteDto>.Failure("Monthly compensation is too large to calculate safely.");
        }

        return Result<HiringQuoteDto>.Success(new HiringQuoteDto(
            country.IsoCode,
            country.Name,
            request.EmploymentType,
            request.MonthlyCompensation,
            estimatedEmployerCosts,
            platformFee,
            total,
            currency,
            country.PayrollLeadTimeDays + 7,
            country.RequiredDocuments.ToArray()));
    }
}
