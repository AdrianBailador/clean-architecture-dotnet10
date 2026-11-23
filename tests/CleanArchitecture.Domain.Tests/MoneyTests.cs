using CleanArchitecture.Domain;

namespace CleanArchitecture.Domain.Tests;

public class MoneyTests
{
    [Fact]
    public void Constructor_throws_for_negative_amount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Money(-1m, "USD"));
    }

    [Theory]
    [InlineData("US")]
    [InlineData("DOLLARS")]
    [InlineData("")]
    public void Constructor_throws_for_invalid_currency_code(string currency)
    {
        Assert.Throws<ArgumentException>(() => new Money(10m, currency));
    }

    [Fact]
    public void Add_throws_when_currencies_differ()
    {
        var usd = new Money(10m, "USD");
        var eur = new Money(10m, "EUR");

        Assert.Throws<InvalidOperationException>(() => usd.Add(eur));
    }

    [Fact]
    public void Add_sums_amounts_of_the_same_currency()
    {
        var result = new Money(10m, "USD").Add(new Money(5m, "USD"));

        Assert.Equal(15m, result.Amount);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public void ApplyDiscount_reduces_amount_by_percentage()
    {
        var result = new Money(200m, "USD").ApplyDiscount(10m);

        Assert.Equal(180m, result.Amount);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void ApplyDiscount_throws_for_out_of_range_percentage(decimal percentage)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Money(100m, "USD").ApplyDiscount(percentage));
    }
}
