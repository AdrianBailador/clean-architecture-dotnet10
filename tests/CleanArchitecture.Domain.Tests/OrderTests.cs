using CleanArchitecture.Domain;

namespace CleanArchitecture.Domain.Tests;

public class OrderTests
{
    private static Order CreateOrder() => new(Guid.NewGuid(), "customer@example.com");

    [Fact]
    public void Constructor_throws_for_invalid_email()
    {
        Assert.Throws<ArgumentException>(() => new Order(Guid.NewGuid(), "not-an-email"));
    }

    [Fact]
    public void Place_throws_when_order_has_no_lines()
    {
        var order = CreateOrder();

        Assert.Throws<InvalidOperationException>(order.Place);
    }

    [Fact]
    public void Place_succeeds_once_order_has_at_least_one_line()
    {
        var order = CreateOrder();
        order.AddLine("Keyboard", 1, new Money(50m, "USD"));

        order.Place();

        Assert.Equal(OrderStatus.Placed, order.Status);
    }

    [Fact]
    public void AddLine_throws_once_order_is_placed()
    {
        var order = CreateOrder();
        order.AddLine("Keyboard", 1, new Money(50m, "USD"));
        order.Place();

        Assert.Throws<InvalidOperationException>(() => order.AddLine("Mouse", 1, new Money(20m, "USD")));
    }

    [Fact]
    public void Ship_throws_when_order_has_not_been_placed()
    {
        var order = CreateOrder();
        order.AddLine("Keyboard", 1, new Money(50m, "USD"));

        Assert.Throws<InvalidOperationException>(order.Ship);
    }

    [Fact]
    public void Ship_succeeds_once_order_is_placed()
    {
        var order = CreateOrder();
        order.AddLine("Keyboard", 1, new Money(50m, "USD"));
        order.Place();

        order.Ship();

        Assert.Equal(OrderStatus.Shipped, order.Status);
    }

    [Fact]
    public void Total_applies_no_discount_below_the_loyalty_threshold()
    {
        var order = CreateOrder();
        order.AddLine("Mouse", 1, new Money(99.99m, "USD"));

        Assert.Equal(99.99m, order.Total().Amount);
    }

    [Fact]
    public void Total_applies_ten_percent_discount_at_or_above_the_loyalty_threshold()
    {
        var order = CreateOrder();
        order.AddLine("Monitor", 1, new Money(100m, "USD"));

        Assert.Equal(90m, order.Total().Amount);
    }
}
