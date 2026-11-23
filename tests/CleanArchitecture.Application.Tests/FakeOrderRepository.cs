using CleanArchitecture.Application.Ports;
using CleanArchitecture.Domain;

namespace CleanArchitecture.Application.Tests;

/// <summary>
/// A test double for IOrderRepository. Because the port lives in the
/// Application layer, testing the use case never needs EF Core, a database,
/// or anything from the Infrastructure project.
/// </summary>
public sealed class FakeOrderRepository : IOrderRepository
{
    private readonly Dictionary<Guid, Order> _orders = new();

    public Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        _orders[order.Id] = order;
        return Task.CompletedTask;
    }

    public Task<Order?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_orders.GetValueOrDefault(id));
}
