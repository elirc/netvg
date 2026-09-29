using PeblClone.Application.Common;

namespace PeblClone.Application.Workforce;

public interface IHiringQuoteCalculator
{
    Task<Result<HiringQuoteDto>> CalculateAsync(HiringQuoteRequest request, CancellationToken cancellationToken);
}
