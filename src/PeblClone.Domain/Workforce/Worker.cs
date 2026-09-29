namespace PeblClone.Domain.Workforce;

public sealed record Worker(
    Guid Id,
    string FullName,
    string Role,
    string CountryIsoCode,
    EmploymentType EmploymentType,
    WorkerStatus Status,
    Money MonthlyCost,
    DateOnly StartDate,
    IReadOnlyList<OnboardingMilestone> OnboardingMilestones,
    IReadOnlyList<ComplianceCheck> ComplianceChecks)
{
    public int OnboardingProgress =>
        OnboardingMilestones.Count == 0
            ? 100
            : (int)Math.Round(OnboardingMilestones.Count(milestone => milestone.IsComplete) * 100m / OnboardingMilestones.Count);

    public bool RequiresAttention =>
        Status == WorkerStatus.NeedsAttention ||
        ComplianceChecks.Any(check => !check.IsResolved && check.RiskLevel is ComplianceRiskLevel.High);
}
