namespace PeblClone.Domain.Workforce;

public sealed record PayrollRun(
    string Id,
    string CountryIsoCode,
    DateOnly PeriodStart,
    DateOnly PeriodEnd,
    DateOnly ApprovalDeadline,
    Money GrossPayroll,
    int WorkerCount,
    bool IsApproved);
