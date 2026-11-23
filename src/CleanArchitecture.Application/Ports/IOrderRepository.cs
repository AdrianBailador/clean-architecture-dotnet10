using CleanArchitecture.Domain;

namespace CleanArchitecture.Application.Ports;

/// <summary>
/// A port: the Application layer defines what it needs from persistence,
/// the Infrastructure layer provides it. Nothing in here knows about EF Core,
/// SQL, or any other storage detail.
/// </summary>
public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken = default);

    Task<Order?> GetAsync(Guid id, CancellationToken cancellationToken = default);
}
