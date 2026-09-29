namespace PeblClone.Domain.Workforce;

public sealed record ComplianceCheck(
    string Id,
    string Title,
    ComplianceRiskLevel RiskLevel,
    string CountryIsoCode,
    string Owner,
    DateOnly DueDate,
    bool IsResolved);
