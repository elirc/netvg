namespace PeblClone.Domain.Workforce;

public sealed record Money(decimal Amount, string Currency)
{
    public static Money Zero(string currency) => new(0, currency);

    public Money Add(Money other)
    {
        if (!Currency.Equals(other.Currency, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Money values must use the same currency.");
        }

        return this with { Amount = Amount + other.Amount };
    }
}
