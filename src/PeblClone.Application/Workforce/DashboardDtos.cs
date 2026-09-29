using PeblClone.Domain.Workforce;

namespace PeblClone.Application.Workforce;

public sealed record WorkforceDashboardDto(
    WorkforceSummaryDto Summary,
    IReadOnlyList<WorkerSummaryDto> Workers,
    IReadOnlyList<CountryReadinessDto> Countries,
    IReadOnlyList<ComplianceAlertDto> ComplianceAlerts,
    IReadOnlyList<PayrollRunDto> PayrollRuns,
    IReadOnlyList<ActivityItemDto> Activity);

public sealed record WorkforceSummaryDto(
    int TotalWorkers,
    int ActiveCountries,
    int WorkersNeedingAttention,
    int OpenComplianceItems,
    decimal MonthlyPayrollUsd,
    int AverageComplianceScore);

public sealed record WorkerSummaryDto(
    Guid Id,
    string FullName,
    string Role,
    string Country,
    string CountryIsoCode,
    EmploymentType EmploymentType,
    WorkerStatus Status,
    decimal MonthlyCost,
    string Currency,
    int OnboardingProgress,
    bool RequiresAttention);

public sealed record CountryReadinessDto(
    string IsoCode,
    string Name,
    string Region,
    int ComplianceScore,
    int PayrollLeadTimeDays,
    bool EmployerOfRecordAvailable,
    bool ContractorManagementAvailable,
    IReadOnlyList<string> RequiredDocuments,
    IReadOnlyList<string> StatutoryBenefits);

public sealed record ComplianceAlertDto(
    string Id,
    string Title,
    ComplianceRiskLevel RiskLevel,
    string Country,
    string Owner,
    DateOnly DueDate);

public sealed record PayrollRunDto(
    string Id,
    string Country,
    DateOnly PeriodEnd,
    DateOnly ApprovalDeadline,
    decimal GrossPayroll,
    string Currency,
    int WorkerCount,
    bool IsApproved);

public sealed record ActivityItemDto(
    string Id,
    string Title,
    string Description,
    DateOnly EventDate,
    string Category);
