namespace CleanArchitecture.Domain;

/// <summary>
/// A value object. Two instances with the same amount and currency are equal,
/// and every operation returns a new instance instead of mutating in place.
/// A class, not a struct, because EF Core's owned-entity mapping (used for
/// this exact kind of value object) requires a reference type.
/// </summary>
public sealed record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public static readonly Money Zero = new(0m, "USD");

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Money cannot be negative.");

        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            throw new ArgumentException("Currency must be a 3-letter ISO code.", nameof(currency));

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Multiply(int factor)
    {
        if (factor < 0)
            throw new ArgumentOutOfRangeException(nameof(factor));

        return new Money(Amount * factor, Currency);
    }

    public Money ApplyDiscount(decimal percentage)
    {
        if (percentage is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(percentage), "Discount must be between 0 and 100.");

        return new Money(Amount * (1 - percentage / 100m), Currency);
    }

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot combine {Currency} with {other.Currency}.");
    }

    public override string ToString() => $"{Amount:0.00} {Currency}";
}
