using CleanArchitecture.Application.UseCases.PlaceOrder;

namespace CleanArchitecture.Application.Tests;

public class PlaceOrderInteractorTests
{
    [Fact]
    public async Task ExecuteAsync_persists_the_order_and_returns_its_total()
    {
        var repository = new FakeOrderRepository();
        var useCase = new PlaceOrderInteractor(repository);

        var request = new PlaceOrderRequest(
            "customer@example.com",
            [new PlaceOrderLineRequest("Keyboard", 2, 20m)]);

        var result = await useCase.ExecuteAsync(request);

        Assert.Equal(40m, result.Total);
        Assert.Equal("Placed", result.Status);

        var stored = await repository.GetAsync(result.OrderId);
        Assert.NotNull(stored);
    }

    [Fact]
    public async Task ExecuteAsync_applies_the_loyalty_discount_through_the_domain_rule()
    {
        var useCase = new PlaceOrderInteractor(new FakeOrderRepository());

        var request = new PlaceOrderRequest(
            "customer@example.com",
            [new PlaceOrderLineRequest("Monitor", 1, 150m)]);

        var result = await useCase.ExecuteAsync(request);

        Assert.Equal(135m, result.Total);
    }

    [Fact]
    public async Task ExecuteAsync_throws_when_the_request_has_no_lines()
    {
        var useCase = new PlaceOrderInteractor(new FakeOrderRepository());
        var request = new PlaceOrderRequest("customer@example.com", []);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(request));
    }
}
