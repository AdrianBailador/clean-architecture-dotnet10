using CleanArchitecture.Application.Ports;
using CleanArchitecture.Domain;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Persistence;

/// <summary>
/// The adapter. This is the only class in the solution that knows the
/// Application layer's IOrderRepository port is backed by EF Core.
/// </summary>
public sealed class EfOrderRepository : IOrderRepository
{
    private readonly CleanArchitectureDbContext _context;

    public EfOrderRepository(CleanArchitectureDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Order?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Orders
            .Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
}
