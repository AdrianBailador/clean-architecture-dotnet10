using CleanArchitecture.Application.Ports;
using CleanArchitecture.Domain;

namespace CleanArchitecture.Application.UseCases.PlaceOrder;

/// <summary>
/// Orchestrates the use case: builds the domain object, lets it enforce its
/// own rules, and persists the result through a port. No business rule lives
/// in this class — if you find yourself adding an `if` about discounts or
/// order validity here, it belongs on <see cref="Order"/> instead.
/// </summary>
public sealed class PlaceOrderInteractor : IPlaceOrderUseCase
{
    private readonly IOrderRepository _orders;

    public PlaceOrderInteractor(IOrderRepository orders)
    {
        _orders = orders;
    }

    public async Task<PlaceOrderResult> ExecuteAsync(
        PlaceOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var order = new Order(Guid.NewGuid(), request.CustomerEmail);

        foreach (var line in request.Lines)
            order.AddLine(line.ProductName, line.Quantity, new Money(line.UnitPrice, request.Currency));

        order.Place();

        await _orders.AddAsync(order, cancellationToken);

        var total = order.Total();
        return new PlaceOrderResult(order.Id, total.Amount, total.Currency, order.Status.ToString());
    }
}
