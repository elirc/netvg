using PeblClone.Application.Workforce;
using PeblClone.Domain.Workforce;

namespace PeblClone.Infrastructure.Workforce;

public sealed class InMemoryWorkforceReadModel : IWorkforceReadModel
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);

    private static readonly IReadOnlyList<CountryProfile> Countries =
    [
        new(
            "DE",
            "Germany",
            "Europe",
            EmployerOfRecordAvailable: true,
            ContractorManagementAvailable: true,
            PayrollLeadTimeDays: 12,
            ComplianceScore: 94,
            PrimaryCurrency: "EUR",
            RequiredDocuments: ["Passport or national ID", "Signed local employment agreement", "Tax identification number"],
            StatutoryBenefits: ["Public health insurance", "Pension contribution", "Paid annual leave"]),
        new(
            "BR",
            "Brazil",
            "Latin America",
            EmployerOfRecordAvailable: true,
            ContractorManagementAvailable: true,
            PayrollLeadTimeDays: 15,
            ComplianceScore: 88,
            PrimaryCurrency: "BRL",
            RequiredDocuments: ["CPF tax ID", "Work eligibility attestation", "Employment agreement"],
            StatutoryBenefits: ["13th salary", "FGTS contribution", "Meal allowance policy"]),
        new(
            "JP",
            "Japan",
            "Asia Pacific",
            EmployerOfRecordAvailable: true,
            ContractorManagementAvailable: false,
            PayrollLeadTimeDays: 18,
            ComplianceScore: 91,
            PrimaryCurrency: "JPY",
            RequiredDocuments: ["Residence card", "My Number registration", "Employment agreement"],
            StatutoryBenefits: ["Health insurance", "Employee pension", "Employment insurance"]),
        new(
            "CA",
            "Canada",
            "North America",
            EmployerOfRecordAvailable: true,
            ContractorManagementAvailable: true,
            PayrollLeadTimeDays: 9,
            ComplianceScore: 96,
            PrimaryCurrency: "CAD",
            RequiredDocuments: ["Government ID", "Social insurance number", "Province-specific tax forms"],
            StatutoryBenefits: ["CPP/QPP", "Employment insurance", "Provincial leave rules"]),
        new(
            "IN",
            "India",
            "Asia Pacific",
            EmployerOfRecordAvailable: true,
            ContractorManagementAvailable: true,
            PayrollLeadTimeDays: 11,
            ComplianceScore: 86,
            PrimaryCurrency: "INR",
            RequiredDocuments: ["PAN", "Aadhaar or passport", "Local contract"],
            StatutoryBenefits: ["Provident fund", "Gratuity eligibility", "State-specific shops act compliance"])
    ];

    private static readonly IReadOnlyList<Worker> Workers =
    [
        new(
            Guid.Parse("df8b2ea2-03b7-4c38-9a7b-2595fe17d3f5"),
            "Nina Schneider",
            "Senior Product Designer",
            "DE",
            EmploymentType.EmployerOfRecord,
            WorkerStatus.Active,
            new Money(11250, "USD"),
            Today.AddDays(-110),
            [
                new("Contract packet", Today.AddDays(-125), true),
                new("Benefits enrollment", Today.AddDays(-100), true),
                new("Payroll profile", Today.AddDays(-95), true)
            ],
            [
                new("DE-BEN-001", "Annual benefit election confirmation", ComplianceRiskLevel.Low, "DE", "People Ops", Today.AddDays(20), false)
            ]),
        new(
            Guid.Parse("57d3718c-7f34-48d5-a9ef-13cc9a50c177"),
            "Mateus Oliveira",
            "Customer Success Lead",
            "BR",
            EmploymentType.EmployerOfRecord,
            WorkerStatus.NeedsAttention,
            new Money(8900, "USD"),
            Today.AddDays(-42),
            [
                new("Contract packet", Today.AddDays(-55), true),
                new("Benefits enrollment", Today.AddDays(-35), false),
                new("Payroll profile", Today.AddDays(-30), true)
            ],
            [
                new("BR-PAY-014", "13th salary accrual review required", ComplianceRiskLevel.High, "BR", "Payroll", Today.AddDays(3), false),
                new("BR-TAX-090", "CPF validation pending", ComplianceRiskLevel.Medium, "BR", "Compliance", Today.AddDays(8), false)
            ]),
        new(
            Guid.Parse("7398f342-1f9e-48ef-a551-09d225868ce6"),
            "Aiko Tanaka",
            "Engineering Manager",
            "JP",
            EmploymentType.EmployerOfRecord,
            WorkerStatus.Onboarding,
            new Money(12800, "USD"),
            Today.AddDays(18),
            [
                new("Contract packet", Today.AddDays(-4), true),
                new("Benefits enrollment", Today.AddDays(6), false),
                new("Payroll profile", Today.AddDays(10), false)
            ],
            [
                new("JP-AUTH-032", "Residence card image awaiting approval", ComplianceRiskLevel.Medium, "JP", "Mobility", Today.AddDays(6), false)
            ]),
        new(
            Guid.Parse("92f3ac13-9235-4a8a-a886-13b6aa9ce051"),
            "Priya Menon",
            "Data Analyst",
            "IN",
            EmploymentType.Contractor,
            WorkerStatus.Active,
            new Money(5200, "USD"),
            Today.AddDays(-78),
            [
                new("Classification questionnaire", Today.AddDays(-95), true),
                new("Statement of work", Today.AddDays(-90), true),
                new("Payment account", Today.AddDays(-82), true)
            ],
            [
                new("IN-CLS-203", "Quarterly contractor classification refresh", ComplianceRiskLevel.Low, "IN", "Legal", Today.AddDays(17), false)
            ]),
        new(
            Guid.Parse("aa6fcbf9-efc5-42d5-9ebe-e7f1b8b4fd52"),
            "Ethan Brooks",
            "Revenue Operations Manager",
            "CA",
            EmploymentType.PayrollOnly,
            WorkerStatus.Active,
            new Money(10300, "USD"),
            Today.AddDays(-160),
            [
                new("Payroll profile", Today.AddDays(-170), true),
                new("Provincial tax setup", Today.AddDays(-168), true)
            ],
            [])
    ];

    private static readonly IReadOnlyList<PayrollRun> PayrollRuns =
    [
        new("RUN-DE-2026-06", "DE", Today.AddDays(-24), Today.AddDays(6), Today.AddDays(2), new Money(11250, "USD"), 1, false),
        new("RUN-BR-2026-06", "BR", Today.AddDays(-24), Today.AddDays(6), Today.AddDays(1), new Money(8900, "USD"), 1, false),
        new("RUN-CA-2026-06", "CA", Today.AddDays(-24), Today.AddDays(6), Today.AddDays(4), new Money(10300, "USD"), 1, true),
        new("RUN-IN-2026-06", "IN", Today.AddDays(-24), Today.AddDays(6), Today.AddDays(3), new Money(5200, "USD"), 1, true)
    ];

    public Task<IReadOnlyList<CountryProfile>> GetCountriesAsync(CancellationToken cancellationToken) =>
        Task.FromResult(Countries);

    public Task<IReadOnlyList<Worker>> GetWorkersAsync(CancellationToken cancellationToken) =>
        Task.FromResult(Workers);

    public Task<IReadOnlyList<PayrollRun>> GetPayrollRunsAsync(CancellationToken cancellationToken) =>
        Task.FromResult(PayrollRuns);
}
