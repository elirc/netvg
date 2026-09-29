using PeblClone.Application.Common;
using PeblClone.Domain.Workforce;

namespace PeblClone.Application.Workforce;

public sealed class HiringQuoteCalculator(IWorkforceReadModel readModel) : IHiringQuoteCalculator
{
    public async Task<Result<HiringQuoteDto>> CalculateAsync(
        HiringQuoteRequest request,
        CancellationToken cancellationToken)
    {
        if (request.MonthlyCompensation <= 0)
        {
            return Result<HiringQuoteDto>.Failure("Monthly compensation must be greater than zero.");
        }

        var countries = await readModel.GetCountriesAsync(cancellationToken);
        var country = countries.SingleOrDefault(item =>
            item.IsoCode.Equals(request.CountryIsoCode, StringComparison.OrdinalIgnoreCase));

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
            _ => 0.12m
        };

        var platformFee = request.EmploymentType switch
        {
            EmploymentType.EmployerOfRecord => 699m,
            EmploymentType.Contractor => 149m,
            EmploymentType.PayrollOnly => 299m,
            _ => 299m
        };

        var estimatedEmployerCosts = Math.Round(request.MonthlyCompensation * employerCostRate, 2);
        var total = request.MonthlyCompensation + estimatedEmployerCosts + platformFee;

        return Result<HiringQuoteDto>.Success(new HiringQuoteDto(
            country.IsoCode,
            country.Name,
            request.EmploymentType,
            request.MonthlyCompensation,
            estimatedEmployerCosts,
            platformFee,
            total,
            request.Currency,
            country.PayrollLeadTimeDays + 7,
            country.RequiredDocuments));
    }
}
