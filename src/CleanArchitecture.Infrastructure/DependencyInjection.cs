using CleanArchitecture.Application.Ports;
using CleanArchitecture.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Uses EF Core's InMemory provider so the sample runs with zero setup.
    /// Swap this one line for <c>UseSqlServer(...)</c> or <c>UseNpgsql(...)</c>
    /// and nothing above the Infrastructure layer has to change.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services) =>
        services
            .AddDbContext<CleanArchitectureDbContext>(options =>
                options.UseInMemoryDatabase("CleanArchitectureDb"))
            .AddScoped<IOrderRepository, EfOrderRepository>();
}
