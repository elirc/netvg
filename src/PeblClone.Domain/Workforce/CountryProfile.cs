namespace PeblClone.Domain.Workforce;

public sealed record CountryProfile(
    string IsoCode,
    string Name,
    string Region,
    bool EmployerOfRecordAvailable,
    bool ContractorManagementAvailable,
    int PayrollLeadTimeDays,
    int ComplianceScore,
    string PrimaryCurrency,
    IReadOnlyList<string> RequiredDocuments,
    IReadOnlyList<string> StatutoryBenefits);
