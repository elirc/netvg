using PeblClone.Domain.Workforce;

namespace PeblClone.Application.Workforce;

public interface IWorkforceReadModel
{
    Task<IReadOnlyList<CountryProfile>> GetCountriesAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Worker>> GetWorkersAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<PayrollRun>> GetPayrollRunsAsync(CancellationToken cancellationToken);
}
