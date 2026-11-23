namespace CleanArchitecture.Application.UseCases.PlaceOrder;

public sealed record PlaceOrderLineRequest(string ProductName, int Quantity, decimal UnitPrice);

public sealed record PlaceOrderRequest(
    string CustomerEmail,
    IReadOnlyList<PlaceOrderLineRequest> Lines,
    string Currency = "USD");

public sealed record PlaceOrderResult(Guid OrderId, decimal Total, string Currency, string Status);
