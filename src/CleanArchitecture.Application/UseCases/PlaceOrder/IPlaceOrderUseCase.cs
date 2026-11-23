namespace CleanArchitecture.Application.UseCases.PlaceOrder;

/// <summary>
/// The boundary: this is the only thing the Api project is allowed to depend
/// on to place an order. It has no idea the interactor behind it exists.
/// </summary>
public interface IPlaceOrderUseCase
{
    Task<PlaceOrderResult> ExecuteAsync(PlaceOrderRequest request, CancellationToken cancellationToken = default);
}
