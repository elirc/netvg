using PeblClone.Domain.Workforce;

namespace PeblClone.Application.Workforce;

public sealed class WorkforceDashboardService(IWorkforceReadModel readModel)
{
    public async Task<WorkforceDashboardDto> GetDashboardAsync(CancellationToken cancellationToken)
    {
        var countries = await readModel.GetCountriesAsync(cancellationToken);
        var workers = await readModel.GetWorkersAsync(cancellationToken);
        var payrollRuns = await readModel.GetPayrollRunsAsync(cancellationToken);
        var countryByIso = countries.ToDictionary(country => country.IsoCode, StringComparer.OrdinalIgnoreCase);

        var openComplianceItems = workers
            .SelectMany(worker => worker.ComplianceChecks)
            .Where(check => !check.IsResolved)
            .ToList();

        var activeCountryCodes = workers.Select(worker => worker.CountryIsoCode).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var summary = new WorkforceSummaryDto(
            TotalWorkers: workers.Count,
            ActiveCountries: activeCountryCodes.Count,
            WorkersNeedingAttention: workers.Count(worker => worker.RequiresAttention),
            OpenComplianceItems: openComplianceItems.Count,
            MonthlyPayrollUsd: workers.Sum(worker => worker.MonthlyCost.Amount),
            AverageComplianceScore: activeCountryCodes.Count == 0
                ? 100
                : (int)Math.Round(activeCountryCodes.Average(code => countryByIso.GetValueOrDefault(code)?.ComplianceScore ?? 0)));

        return new WorkforceDashboardDto(
            Summary: summary,
            Workers: workers
                .OrderByDescending(worker => worker.RequiresAttention)
                .ThenBy(worker => worker.FullName)
                .Select(worker => MapWorker(worker, countryByIso))
                .ToList(),
            Countries: countries
                .OrderByDescending(country => activeCountryCodes.Contains(country.IsoCode, StringComparer.OrdinalIgnoreCase))
                .ThenByDescending(country => country.ComplianceScore)
                .Select(MapCountry)
                .ToList(),
            ComplianceAlerts: openComplianceItems
                .OrderByDescending(check => check.RiskLevel)
                .ThenBy(check => check.DueDate)
                .Select(check => MapCompliance(check, countryByIso))
                .ToList(),
            PayrollRuns: payrollRuns
                .OrderBy(run => run.ApprovalDeadline)
                .Select(run => MapPayroll(run, countryByIso))
                .ToList(),
            Activity: BuildActivity(workers));
    }

    private static WorkerSummaryDto MapWorker(
        Worker worker,
        IReadOnlyDictionary<string, CountryProfile> countryByIso)
    {
        var country = countryByIso.GetValueOrDefault(worker.CountryIsoCode);
        return new WorkerSummaryDto(
            worker.Id,
            worker.FullName,
            worker.Role,
            country?.Name ?? worker.CountryIsoCode,
            worker.CountryIsoCode,
            worker.EmploymentType,
            worker.Status,
            worker.MonthlyCost.Amount,
            worker.MonthlyCost.Currency,
            worker.OnboardingProgress,
            worker.RequiresAttention);
    }

    private static CountryReadinessDto MapCountry(CountryProfile country) =>
        new(
            country.IsoCode,
            country.Name,
            country.Region,
            country.ComplianceScore,
            country.PayrollLeadTimeDays,
            country.EmployerOfRecordAvailable,
            country.ContractorManagementAvailable,
            country.RequiredDocuments,
            country.StatutoryBenefits);

    private static ComplianceAlertDto MapCompliance(
        ComplianceCheck check,
        IReadOnlyDictionary<string, CountryProfile> countryByIso) =>
        new(
            check.Id,
            check.Title,
            check.RiskLevel,
            countryByIso.GetValueOrDefault(check.CountryIsoCode)?.Name ?? check.CountryIsoCode,
            check.Owner,
            check.DueDate);

    private static PayrollRunDto MapPayroll(
        PayrollRun run,
        IReadOnlyDictionary<string, CountryProfile> countryByIso) =>
        new(
            run.Id,
            countryByIso.GetValueOrDefault(run.CountryIsoCode)?.Name ?? run.CountryIsoCode,
            run.PeriodEnd,
            run.ApprovalDeadline,
            run.GrossPayroll.Amount,
            run.GrossPayroll.Currency,
            run.WorkerCount,
            run.IsApproved);

    private static IReadOnlyList<ActivityItemDto> BuildActivity(IReadOnlyList<Worker> workers) =>
        workers
            .OrderByDescending(worker => worker.StartDate)
            .Take(5)
            .Select(worker => new ActivityItemDto(
                worker.Id.ToString("N"),
                $"{worker.FullName} moved to {worker.Status}",
                $"{worker.Role} in {worker.CountryIsoCode} via {worker.EmploymentType}",
                worker.StartDate,
                worker.RequiresAttention ? "attention" : "workforce"))
            .ToList();
}
