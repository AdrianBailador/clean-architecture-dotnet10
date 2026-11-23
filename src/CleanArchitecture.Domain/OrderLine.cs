namespace CleanArchitecture.Domain;

public sealed class OrderLine
{
    public string ProductName { get; }
    public int Quantity { get; }
    public Money UnitPrice { get; }
    public Money LineTotal => UnitPrice.Multiply(Quantity);

    /// <summary>
    /// For EF Core only. UnitPrice is an owned reference, not a scalar column,
    /// so EF cannot bind it through the constructor below — it falls back to
    /// this one and sets the properties directly through their backing fields.
    /// </summary>
    private OrderLine()
    {
        ProductName = string.Empty;
        UnitPrice = Money.Zero;
    }

    public OrderLine(string productName, int quantity, Money unitPrice)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name is required.", nameof(productName));

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");

        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
