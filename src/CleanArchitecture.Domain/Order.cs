namespace CleanArchitecture.Domain;

public enum OrderStatus
{
    Draft,
    Placed,
    Shipped,
}

/// <summary>
/// The aggregate root. Every rule about what makes an order valid lives here,
/// not in a service, a validator, or a controller.
/// </summary>
public sealed class Order
{
    private const decimal LoyaltyDiscountThreshold = 100m;
    private const decimal LoyaltyDiscountPercentage = 10m;

    private readonly List<OrderLine> _lines = new();

    public Guid Id { get; }
    public string CustomerEmail { get; }
    public OrderStatus Status { get; private set; } = OrderStatus.Draft;
    public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();

    public Order(Guid id, string customerEmail)
    {
        if (string.IsNullOrWhiteSpace(customerEmail) || !customerEmail.Contains('@'))
            throw new ArgumentException("A valid customer email is required.", nameof(customerEmail));

        Id = id;
        CustomerEmail = customerEmail;
    }

    public void AddLine(string productName, int quantity, Money unitPrice)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Cannot add lines to an order that has already been placed.");

        _lines.Add(new OrderLine(productName, quantity, unitPrice));
    }

    public Money Subtotal() =>
        _lines.Aggregate(Money.Zero, (total, line) => total.Add(line.LineTotal));

    /// <summary>
    /// The one rule in this whole sample worth pointing at: orders of $100 or
    /// more get a 10% loyalty discount, applied here and nowhere else.
    /// </summary>
    public Money Total()
    {
        var subtotal = Subtotal();
        return subtotal.Amount >= LoyaltyDiscountThreshold
            ? subtotal.ApplyDiscount(LoyaltyDiscountPercentage)
            : subtotal;
    }

    public void Place()
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException($"Cannot place an order that is already {Status}.");

        if (_lines.Count == 0)
            throw new InvalidOperationException("Cannot place an order with no lines.");

        Status = OrderStatus.Placed;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Placed)
            throw new InvalidOperationException($"Cannot ship an order that is {Status}; it must be Placed first.");

        Status = OrderStatus.Shipped;
    }
}
